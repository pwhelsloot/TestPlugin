using Newtonsoft.Json;

namespace AMCS.Data.Server.Api
{
  public class ApiResult<T>
  {
    [JsonProperty(PropertyName = "resource")]
    public T Resource { get; set; }

    public ApiResultExtra Extra { get; set; }
  }
}