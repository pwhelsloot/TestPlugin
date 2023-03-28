using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Controllers.Responses
{
  public class LoginProviderConfigurationResponse
  {
    [JsonProperty("provider")]
    public string Provider { get; set; }

    [JsonProperty("providerName")]
    public string ProviderName { get; set; }

    [JsonProperty("loginUrl")]
    public string LoginUrl { get; set; }

    [JsonProperty("logoutUrl")]
    public string LogoutUrl { get; set; }

    [JsonProperty("isExternalLogoutRequired")]
    public bool IsExternalLogoutRequired { get; set; }
  }
}
