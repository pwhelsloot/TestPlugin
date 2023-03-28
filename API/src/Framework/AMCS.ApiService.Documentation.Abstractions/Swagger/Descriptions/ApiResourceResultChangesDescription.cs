using System.Collections.Generic;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(ContractProvider = typeof(ApiResourceResultContractProvider))]
  public class ApiResourceResultChangesDescription<T> : ApiResourceDescription
  {
    [JsonProperty("resource")]
    public List<T> Resource { get; set; }

    [JsonProperty("extra")]
    public ApiResourceResultChangesExtraDescription Extra { get; set; }
  }
}
