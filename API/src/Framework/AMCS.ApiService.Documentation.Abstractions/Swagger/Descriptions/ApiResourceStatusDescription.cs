using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  [ApiExplorer(Name = "ApiResourceStatus")]
  public class ApiResourceStatusDescription
  {
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }
  }
}
