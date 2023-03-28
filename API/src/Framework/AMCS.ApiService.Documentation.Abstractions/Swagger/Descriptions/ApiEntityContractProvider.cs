using System;
using System.Collections.Generic;
using System.Reflection;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using Newtonsoft.Json.Serialization;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  // This contract provider generates contracts for ApiEntityDescription`1
  // classes. This class is a marker type to trigger the contract generation
  // behavior implemented here. We specifically do not implement a complete
  // Newtonsoft.Json contract and what's generated here will not be usable
  // by real (de)serialization. Instead, it's enough for Swagger to generate
  // proper metadata for our entity controllers.
  internal class ApiEntityContractProvider : IApiExplorerContractProvider
  {
    // To differentiate plain EntityObject's and custom EntityObject's,
    // we postfix type names that use EntityObjectAccessor serialization
    // with Resource. ApiTaxBreakdown becomes ApiTaxBreakdownResource, and
    // CustomerEntity becomes CustomerResource.
    public string GetTypeName(Type type)
    {
      if (!typeof(ApiEntityDescription<>).IsAssignableFromGeneric(type))
        throw new ArgumentException("Expected type to inherit from ApiEntityDescription`1");

      var entityType = type.GetGenericArguments()[0];
      var typeName = entityType.Name;

      var attribute = entityType.GetCustomAttribute<ApiExplorerAttribute>(true);
      if (attribute?.Name != null)
        typeName = attribute.Name;

      const string Postfix = "Entity";
      if (typeName.EndsWith(Postfix))
        typeName = typeName.Substring(0, typeName.Length - Postfix.Length);
      return typeName + "Resource";
    }

    public JsonContract CreateContract(Type type, IContractResolver contractResolver)
    {
      if (!typeof(ApiEntityDescription<>).IsAssignableFromGeneric(type))
        throw new ArgumentException("Expected type to inherit from ApiEntityDescription`1");

      var entityType = type.GetGenericArguments()[0];
      var contract = new JsonObjectContract(entityType);
      var accessor = EntityObjectAccessor.ForType(entityType);

      foreach (var property in accessor.Properties)
      {
        if (property.Column?.CanWrite == true)
        {
          contract.Properties.Add(new JsonProperty
          {
            PropertyName = property.Column.ColumnName,
            UnderlyingName = property.Name,
            PropertyType = WrapEntityType(property.Type),
            DeclaringType = entityType,
            Readable = true,
            Writable = true
          });
        }
      }

      return contract;
    }

    // Wrap a nested EntityObject type in a ApiEntityDescription`1 to recursively
    // trigger custom metadata generation.
    private static Type WrapEntityType(Type propertyType)
    {
      if (typeof(EntityObject).IsAssignableFrom(propertyType))
        return typeof(ApiEntityDescription<>).MakeGenericType(propertyType);

      if (TryWrapEntityListType(propertyType, out var wrappedPropertyType))
        return wrappedPropertyType;

      foreach (var @interface in propertyType.GetInterfaces())
      {
        if (TryWrapEntityListType(@interface, out var wrappedListType))
          return wrappedListType;
      }

      return propertyType;
    }

    // If the provided type either is a IEnumerable<T> where T : EntityObject,
    // or implements one, return a List<ApiEntityDescription<T>>.
    private static bool TryWrapEntityListType(Type type, out Type wrappedType)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
      {
        var elementType = type.GetGenericArguments()[0];
        if (typeof(EntityObject).IsAssignableFrom(elementType))
        {
          wrappedType = typeof(List<>).MakeGenericType(typeof(ApiEntityDescription<>).MakeGenericType(elementType));
          return true;
        }
      }

      wrappedType = null;
      return false;
    }
  }
}
