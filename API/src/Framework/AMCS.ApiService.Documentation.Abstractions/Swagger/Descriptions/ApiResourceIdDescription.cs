using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceId")]
  public class ApiResourceIdDescription : ApiResourceDescription
  {
    [JsonProperty("resource")]
    public int Resource { get; set; }
  }
}
