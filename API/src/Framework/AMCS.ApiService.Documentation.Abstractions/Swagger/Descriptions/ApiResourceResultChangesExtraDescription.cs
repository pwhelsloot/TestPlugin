using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceResultUpdatesExtra")]
  public class ApiResourceResultChangesExtraDescription
  {
    [JsonProperty("until")]
    public string Until { get; set; }

    [JsonProperty("cursor")]
    public string Cursor { get; set; }
  }
}
