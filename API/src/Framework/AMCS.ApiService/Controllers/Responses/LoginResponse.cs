using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Controllers.Responses
{
  public class LoginResponse
  {
    [JsonProperty("authResult")]
    public string AuthResult { get; set; }

    [JsonProperty("username")]
    public string UserName { get; set; }

    [JsonProperty("userIdentity")]
    public string UserIdentity { get; set; }

    [JsonProperty("sysUserId")]
    public int? SysUserId { get; set; }

    [JsonProperty("userGuid")]
    public Guid? UserGuid { get; set; }

    [JsonProperty("companyoutletid")]
    public int? CompanyOutletId { get; set; }

    [JsonProperty("companyOutletGuid")]
    public Guid? CompanyOutletGuid { get; set; }

    [JsonProperty("stayLoggedIn")]
    public bool StayLoggedIn { get; set; }
  }
}
