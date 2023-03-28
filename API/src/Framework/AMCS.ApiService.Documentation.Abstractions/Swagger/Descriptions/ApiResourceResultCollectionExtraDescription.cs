using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceResultCollectionExtra")]
  public class ApiResourceResultCollectionExtraDescription
  {
    [JsonProperty("count")]
    public int? Count { get; set; }
  }
}
