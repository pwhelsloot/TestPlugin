using Newtonsoft.Json;

namespace AMCS.Data.Server.Api
{
  public class ApiResultExtra
  {
    [JsonProperty(PropertyName = "count")]
    public int Count { get; set; }
  }
}