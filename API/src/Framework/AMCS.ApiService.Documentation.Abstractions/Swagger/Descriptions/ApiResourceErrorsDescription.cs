using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceErrors")]
  public class ApiResourceErrorsDescription
  {
    [JsonProperty("errors")]
    public string Errors { get; set; }
  }
}
