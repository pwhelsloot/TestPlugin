using AMCS.Data.SourceGenerator.ExtensionMethods;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AMCS.Data.SourceGenerator.Helpers
{
  internal class PropertyToGenerate
  {
    public string FieldName { get; }

    public string PropertyName { get; }

    public ITypeSymbol TypeSymbol { get; }

    public TypeSyntax TypeSyntax { get; }

    public TypeSyntax InternalRepresentationTypeSyntax { get; }

    public string TypeFullMetadataName { get; }

    public bool IsNullable { get; }

    public bool IsReferenceType => TypeSymbol.IsReferenceType;

    public INamedTypeSymbol? ReferenceEntity { get; }

    public PropertyToGenerate(string fieldName, string propertyName, ITypeSymbol typeSymbol, bool isNullable, INamedTypeSymbol? referenceEntity = null)
    {
      FieldName = fieldName;
      PropertyName = propertyName;
      TypeSymbol = typeSymbol;
      TypeFullMetadataName = typeSymbol.GetFullMetadataName();
      IsNullable = isNullable;

      var typeSyntax = TypeSyntax = ParseTypeName(typeSymbol.ToDisplayString());
      InternalRepresentationTypeSyntax = !(typeSyntax is NullableTypeSyntax) ? NullableType(typeSyntax) : typeSyntax;
      ReferenceEntity = referenceEntity;
    }
  }
}
