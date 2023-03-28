using AMCS.Data.SourceGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.SourceGenerator.ExtensionMethods
{
  internal static class GeneratorExecutionContextExtensionMethods
  {
    private static readonly DiagnosticDescriptor GeneratorOnAbstractDiagnostic = new("DBG001", "Cannot generate entity object builder for abstract class", "Cannot generate data builder for abstract class {0}", "EntityObjectBuilderGenerator", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor GeneratorOnInvalidReferencePropertyDiagnostic = new("DBG002", "Cannot generate reference entity for an invalid property", "Cannot generate reference entity for an invalid {0}", "EntityObjectBuilderReference", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor GeneratorOnInvalidEntityReferenceDiagnostic = new("DBG003", "Cannot generate a builder to an entity without a reference to [EntityTableAttribute]", "{0} is invalid because it does not contains the [EntityTableAttribute] attribute", "EntityTableAttribute", DiagnosticSeverity.Error, true);
    private static readonly string EntityObjectBuilderAttribute = "AMCS.Data.SourceGenerator.Attributes.EntityObjectBuilderAttribute";
    private static readonly string EntityObjectBuilderReferenceAttribute = "AMCS.Data.SourceGenerator.Attributes.EntityObjectBuilderReferenceAttribute";
    private static readonly string EntityTableAttribute = "AMCS.Data.Entity.EntityTableAttribute";

    public static Dictionary<string, BuilderToGenerate> DetermineBuildersToGenerate(this GeneratorExecutionContext context)
    {
      Dictionary<string, BuilderToGenerate> candidates = new();
      var entityObjectBuilderAttribute = context.Compilation.GetTypeByMetadataName(EntityObjectBuilderAttribute)!;
      var referenceBuilderAttribute = context.Compilation.GetTypeByMetadataName(EntityObjectBuilderReferenceAttribute)!;
      var entityTableAttribute = context.Compilation.GetTypeByMetadataName(EntityTableAttribute)!;

      if (entityObjectBuilderAttribute == null) return candidates;

      foreach (var inputDocument in context.Compilation.SyntaxTrees)
      {
        context.CancellationToken.ThrowIfCancellationRequested();

        var typeNodes = inputDocument.GetRoot()
          .DescendantNodesAndSelf(n => n is CompilationUnitSyntax || n is NamespaceDeclarationSyntax || n is TypeDeclarationSyntax)
          .OfType<TypeDeclarationSyntax>();

        foreach (var candidateTypeNode in typeNodes)
        {
          var semanticModel = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);

          if (!candidateTypeNode.AttributeLists.ContainsAttributeType(semanticModel, entityObjectBuilderAttribute, exactMatch: true)) continue;

          if (semanticModel.GetDeclaredSymbol(candidateTypeNode) is INamedTypeSymbol candidateType)
          {
            var entityObjectBuilder = FilterClassAttributes(candidateType, entityObjectBuilderAttribute).FirstOrDefault();

            if (entityObjectBuilder.ConstructorArgument<INamedTypeSymbol>(0) is not INamedTypeSymbol entityType) continue;

            var entityTableAttributeReference = FilterClassAttributes(entityType, entityTableAttribute).FirstOrDefault();
            if (entityTableAttributeReference == null)
            {
              context.ReportDiagnostic(Diagnostic.Create(GeneratorOnInvalidEntityReferenceDiagnostic, Location.None, entityType));
              continue;
            }

            var entityPrimaryKey = entityTableAttributeReference.ConstructorArgument<string>(1)!;

            BuilderToGenerate builder = new(candidateType.ContainingNamespace?.GetFullMetadataName(),
              candidateType.Name,
              candidateTypeNode,
              candidateType.DeclaredAccessibility,
              entityType,
              entityPrimaryKey);

            var defaultValues = entityObjectBuilder.NamedArguments.FirstOrDefault(argName => argName.Key == "DefaultValues");
            if (defaultValues.Key != null && defaultValues.Value.Values != null)
            {
              builder.DefaultValues.AddRange(defaultValues.Value.Values.Select(val =>
                $"{val.Value.ToString().Substring(0, 1).ToLower()}{val.Value.ToString().Substring(1)}"
              ));
            }

            var deDublicateValues = entityObjectBuilder.NamedArguments.FirstOrDefault(argName => argName.Key == "DeDuplicate");
            if (deDublicateValues.Key != null && deDublicateValues.Value.Values != null)
            {
              builder.DeDuplicate.AddRange(deDublicateValues.Value.Values.Select(val => val.Value.ToString()));
            }

            // Filter the builder to check if it has any EntityObjectBuilderReferenceAttribute
            var referenceBuilderAttributes = FilterClassAttributes(candidateType, referenceBuilderAttribute);
            if (referenceBuilderAttributes.Any())
            {
              HandleReferenceEntities(context, semanticModel, entityObjectBuilderAttribute, builder, referenceBuilderAttributes);
            }

            IMethodSymbol? constructorToUse = null;
            foreach (var typeSymbolConstructor in entityType.Constructors)
            {
              if (typeSymbolConstructor.IsStatic) continue;

              if (typeSymbolConstructor.Parameters.Length > (constructorToUse?.Parameters.Length ?? 0))
              {
                constructorToUse = typeSymbolConstructor;
              }
            }

            builder.ConstructorToUse = constructorToUse;

            builder.Properties.AddRange(
              GetEntityTypeProperties(builder.EntityType, builder, context, semanticModel)
            );

            candidates.Add(candidateType.GetFullMetadataName(), builder);
          }
        }
      }

      return candidates;
    }

    private static void HandleReferenceEntities(GeneratorExecutionContext context, SemanticModel semanticModel, INamedTypeSymbol entityObjectBuilderAttribute,
        BuilderToGenerate builder, IEnumerable<AttributeData> referenceBuilderAttributes)
    {
      foreach (var referenceAttribute in referenceBuilderAttributes)
      {
        var property = referenceAttribute.ConstructorArgument<string>(0)!;
        var referenceBuilder = referenceAttribute.ConstructorArgument<INamedTypeSymbol>(1)!;
        var propertySymbol = builder.EntityType.GetPropertyByName(property);

        if (propertySymbol == null)
        {
          var referenceEntityObjectBuilder = FilterClassAttributes(referenceBuilder, entityObjectBuilderAttribute).FirstOrDefault();
          if (referenceEntityObjectBuilder.ConstructorArgument<INamedTypeSymbol>(0) is not INamedTypeSymbol referenceEntityType) continue;
          propertySymbol = referenceEntityType.GetPropertyByName(property);

          if (propertySymbol == null)
          {
            context.ReportDiagnostic(Diagnostic.Create(GeneratorOnInvalidReferencePropertyDiagnostic, Location.None, property));
            continue;
          }
        }

        builder.ReferenceEntities.Add(new PropertyToGenerate(
          $"{propertySymbol!.Name.Substring(0, 1).ToLower()}{propertySymbol.Name.Substring(1)}",
          propertySymbol.Name,
          propertySymbol.Type,
          DetermineNullability(semanticModel, propertySymbol),
          referenceBuilder)
        );
      }
    }

    /// <summary>
    /// Filter all the attributes from <paramref name="classCandidate"/> and return the ones that match the <paramref name="attributeName"/>.<br/>
    /// </summary>
    /// <param name="classCandidate">The class that contains the attributes</param>
    /// <param name="attributeName">The attribute from GetTypeByMetadataName</param>
    /// <returns>A filtered list of attributes from the <paramref name="classCandidate"/>.</returns>
    private static IEnumerable<AttributeData> FilterClassAttributes(INamedTypeSymbol classCandidate, INamedTypeSymbol attributeName)
    {
      return classCandidate.GetAttributes().Where(attribute =>
        attribute.AttributeClass != null &&
        //attribute.ApplicationSyntaxReference != null &&
        attribute.AttributeClass.Equals(attributeName, SymbolEqualityComparer.Default)
      )!;
    }

    private static bool DetermineNullability(SemanticModel semanticModel, IPropertySymbol propertySymbol)
    {
      if (propertySymbol.Type.IsValueType) return propertySymbol.Type.IsNullable();

      var nullableContext = semanticModel.GetNullableContext(propertySymbol.Locations[0].SourceSpan.Start);
      if (nullableContext.AnnotationsEnabled())
      {
        return propertySymbol.DeclaringSyntaxReferences.Length != 0
          && (propertySymbol.DeclaringSyntaxReferences[0].GetSyntax() as PropertyDeclarationSyntax)?.Type is NullableTypeSyntax;
      }

      return false;
    }

    private static IEnumerable<PropertyToGenerate> GetEntityTypeProperties(INamedTypeSymbol entityType, BuilderToGenerate builder, GeneratorExecutionContext context, SemanticModel semanticModel)
    {
      var properties = new List<PropertyToGenerate>();

      if (entityType.BaseType != null &&
        entityType.BaseType.ToString() != "object")
      {
        properties.AddRange(
          GetEntityTypeProperties(entityType.BaseType, builder, context, semanticModel)
        );
      }

      IEnumerable<IPropertySymbol> propertiesWithAttributeSymbol = entityType.GetPropertiesWithAttributeSymbol(context);
      propertiesWithAttributeSymbol = propertiesWithAttributeSymbol.Where(prop =>
        !builder.ReferenceEntities.Any(refer => refer.PropertyName == prop.Name) &&
        prop.Name != "RowVersion" &&
        prop.Name.ToLower() != builder.EntityTypePrimaryKey.ToLower()
      );

      properties.AddRange(propertiesWithAttributeSymbol
        .Where(m => m.DeclaredAccessibility == Accessibility.Public && !m.IsReadOnly && !m.IsIndexer && !m.IsStatic)
        .Select(property => new PropertyToGenerate(
          $"{property.Name.Substring(0, 1).ToLower()}{property.Name.Substring(1)}",
          property.Name,
          property.Type,
          DetermineNullability(semanticModel, property)))
      );

      return properties;
    }
  }
}
