using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AMCS.Data.SourceGenerator.ExtensionMethods
{
  internal static class SyntaxExtensions
  {
    internal static bool ContainsAttributeType(this SyntaxList<AttributeListSyntax> attributes, SemanticModel semanticModel,
      INamedTypeSymbol attributeType, bool exactMatch = false)
    {
      return attributes.Any(list => list.Attributes.Any(attrbute => attributeType.IsAssignableFrom(semanticModel.GetTypeInfo(attrbute).Type!, exactMatch)));
    }

    internal static SyntaxToken[] GetModifiers(this Accessibility accessibility)
    {
      var list = new List<SyntaxToken>(2);

      switch (accessibility)
      {
        case Accessibility.Internal:
          list.Add(Token(SyntaxKind.InternalKeyword));
          break;
        case Accessibility.Public:
          list.Add(Token(SyntaxKind.PublicKeyword));
          break;
        case Accessibility.Private:
          list.Add(Token(SyntaxKind.PrivateKeyword));
          break;
        case Accessibility.Protected:
          list.Add(Token(SyntaxKind.ProtectedKeyword));
          break;
        case Accessibility.ProtectedOrInternal:
          list.Add(Token(SyntaxKind.InternalKeyword));
          list.Add(Token(SyntaxKind.ProtectedKeyword));
          break;
        case Accessibility.ProtectedAndInternal:
          list.Add(Token(SyntaxKind.PrivateKeyword));
          list.Add(Token(SyntaxKind.ProtectedKeyword));
          break;
        case Accessibility.NotApplicable:
          break;
      }

      return list.ToArray();
    }
  }
}
