using System.Collections.Generic;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceResultEntityLinks")]
  public class ApiResourceResultEntityLinksDescription
  {
    [JsonProperty("self")]
    public string Self { get; set; }

    [JsonProperty("associations")]
    public List<string> Associations { get; set; }

    [JsonProperty("expand")]
    public List<string> Expand { get; set; }

    [JsonProperty("operations")]
    public List<string> Operations { get; set; }
  }
}
