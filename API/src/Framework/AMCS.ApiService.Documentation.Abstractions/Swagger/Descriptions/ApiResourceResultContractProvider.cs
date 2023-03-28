using System;
using System.Collections.Generic;
using AMCS.Data;
using Newtonsoft.Json.Serialization;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  internal class ApiResourceResultContractProvider : IApiExplorerContractProvider
  {
    private static readonly Dictionary<Type, Func<Type, string>> TypeNameBuilders = new Dictionary<Type, Func<Type, string>>
    {
      { typeof(ApiResourceResultDescription<>), p => "ApiResourceResult[" + GetSimpleTypeName(p.GetGenericArguments()[0]) + "]" },
      { typeof(ApiResourceResultEntityDescription<>), p => "ApiResourceResultEntity[" + GetSimpleTypeName(p.GetGenericArguments()[0]) + "]" },
      { typeof(ApiResourceResultCollectionDescription<>), p => "ApiResourceResultCollection[" + GetSimpleTypeName(p.GetGenericArguments()[0]) + "]" },
      { typeof(ApiResourceResultChangesDescription<>), p => "ApiResourceChanges[" + GetSimpleTypeName(p.GetGenericArguments()[0]) + "]" },
    };

    public string GetTypeName(Type type)
    {
      if (TypeNameBuilders.TryGetValue(type.GetGenericTypeDefinition(), out var builder))
        return builder(type);
      return null;
    }

    private static string GetSimpleTypeName(Type type)
    {
      if (typeof(ApiEntityDescription<>).IsAssignableFromGeneric(type))
        return new ApiEntityContractProvider().GetTypeName(type);

      return ApiResourceUtils.FriendlyId(type);
    }

    public JsonContract CreateContract(Type type, IContractResolver contractResolver)
    {
      return null;
    }
  }
}
