using Newtonsoft.Json;

namespace AMCS.Data.Server.Api
{
  public class ApiDeleteResult
  {
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "isSuccess")]
    public bool IsSuccess { get; set; }
  }
}