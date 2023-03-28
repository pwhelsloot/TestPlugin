using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.SourceGenerator.ExtensionMethods
{
  internal static class INamedTypeSymbolExtensionMethods
  {
    public static IEnumerable<IPropertySymbol> GetPropertiesWithAttributeSymbol(this INamedTypeSymbol typeSymbol, GeneratorExecutionContext context)
    {
      var dataMemberAttributeSymbol = context.Compilation.GetTypeByMetadataName("System.Runtime.Serialization.DataMemberAttribute");
      var entityMemberAttributeSymbol = context.Compilation.GetTypeByMetadataName("AMCS.Data.Entity.EntityMemberAttribute");

      return typeSymbol.GetMembers().OfType<IPropertySymbol>().Where(member => member.Kind == SymbolKind.Property &&
        member.GetAttributes().Any(attribute =>
          attribute.AttributeClass!.MetadataName == dataMemberAttributeSymbol!.MetadataName ||
          attribute.AttributeClass!.MetadataName == entityMemberAttributeSymbol!.MetadataName
        ));
    }

    /// <summary>
    /// Find propertyName in typeSymbol (Entity, e.g. JobWeighingEntity). 
    /// If not available try to find it in the BaseType (inherited Entity, e.g. WeighingEntity).
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>Property when found</returns>
    public static IPropertySymbol? GetPropertyByName(this INamedTypeSymbol typeSymbol, string propertyName)
    {
      var property = typeSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(member => member.Kind == SymbolKind.Property && member.Name == propertyName);

      if (property == null &&
        typeSymbol.BaseType != null &&
        typeSymbol.BaseType.ToString() != "object")
      {
        property = typeSymbol.BaseType.GetPropertyByName(propertyName);
      }

      return property;
    }
  }
}
