using System;
using Newtonsoft.Json.Serialization;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger
{
  public interface IApiExplorerContractProvider
  {
    string GetTypeName(Type type);

    JsonContract CreateContract(Type type, IContractResolver contractResolver);
  }
}
