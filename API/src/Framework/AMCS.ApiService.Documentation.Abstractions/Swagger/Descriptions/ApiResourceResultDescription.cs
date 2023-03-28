using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(ContractProvider = typeof(ApiResourceResultContractProvider))]
  public class ApiResourceResultDescription<T> : ApiResourceDescription
  {
    [JsonProperty("resource")]
    public T Resource { get; set; }
  }
}
