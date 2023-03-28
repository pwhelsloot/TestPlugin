using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResource")]
  public class ApiResourceDescription
  {
    [JsonProperty("errors")]
    public ApiResourceErrorsDescription Errors { get; set; }

    [JsonProperty("status")]
    public ApiResourceStatusDescription Status { get; set; }
  }
}
