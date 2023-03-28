using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

#nullable enable
namespace AMCS.Data.SourceGenerator.Helpers
{
  internal class BuilderToGenerate
  {
    public string? Namespace { get; }

    public string Name { get; }

    public string BuilderName { get; }

    public TypeSyntax BuilderTypeSyntax { get; }

    public TypeDeclarationSyntax TypeNode { get; }

    public Accessibility Accessibility { get; }

    public IMethodSymbol? ConstructorToUse { get; set; }

    public INamedTypeSymbol EntityType { get; }

    public string EntityTypePrimaryKey { get; }

    public List<PropertyToGenerate> Properties { get; set; }

    public List<PropertyToGenerate> ReferenceEntities { get; set; }

    public List<string> DefaultValues { get; set; }

    public List<string> DeDuplicate { get; set; }

    public BuilderToGenerate(string? @namespace, string name, TypeDeclarationSyntax typeNode, Accessibility accessibility, INamedTypeSymbol entityType, string entityTypePrimaryKey)
    {
      Namespace = @namespace;
      Name = name;
      TypeNode = typeNode;
      BuilderName = name;
      Accessibility = accessibility;
      BuilderTypeSyntax = ParseTypeName(BuilderName);
      EntityType = entityType;
      EntityTypePrimaryKey = entityTypePrimaryKey;
      Properties = new List<PropertyToGenerate>();
      ReferenceEntities = new List<PropertyToGenerate>();
      DefaultValues = new List<string>();
      DeDuplicate = new List<string>();
    }
  }
}
