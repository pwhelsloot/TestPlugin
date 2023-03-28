using System.Collections.Generic;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceResultEntityExtra")]
  public class ApiResourceResultEntityExtraDescription
  {
    // The type here is the proper type, but Swagger outputs this as just {},
    // which kind of makes sense because the whole point here is that the keys
    // aren't available to Swagger.
    [JsonProperty("expand")]
    public Dictionary<string, Dictionary<string, object>> Expand { get; set; }

    // The type here is the proper type, but Swagger outputs this as just {},
    // which kind of makes sense because the whole point here is that the keys
    // aren't available to Swagger.
    [JsonProperty("include")]
    public Dictionary<string, List<Dictionary<string, object>>> Include { get; set; }
  }
}
