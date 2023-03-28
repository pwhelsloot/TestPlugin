using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(ContractProvider = typeof(ApiResourceResultContractProvider))]
  public class ApiResourceResultEntityDescription<T> : ApiResourceDescription
  {
    [JsonProperty("resource")]
    public T Resource { get; set; }

    [JsonProperty("links")]
    public ApiResourceResultEntityLinksDescription Links { get; set; }

    [JsonProperty("extra")]
    public ApiResourceResultEntityExtraDescription Extra { get; set; }
  }
}
