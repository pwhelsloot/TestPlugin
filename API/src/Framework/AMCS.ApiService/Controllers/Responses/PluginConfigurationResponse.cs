namespace AMCS.ApiService.Controllers.Responses
{
  using System.Text.Json.Serialization;
  using Newtonsoft.Json;

  public class PluginConfigurationResponse
  {
    [JsonProperty("mex_endpoint")]
    [JsonPropertyName("mex_endpoint")]
    public string MexEndpoint { get; set;  }

    [JsonProperty("web_hooks_endpoint")]
    [JsonPropertyName("web_hooks_endpoint")]
    public string WebHooksEndpoint { get; set; }

    public PluginConfigurationResponse()
    {
    }

    public PluginConfigurationResponse(string mexEndpoint, string webHookEndpoint)
    {
      MexEndpoint = mexEndpoint;
      WebHooksEndpoint = webHookEndpoint;
    }
  };
}
