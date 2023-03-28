using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Controllers.Responses
{
  public class LogoutResponse
  {
    [JsonProperty("loggedOut")]
    public string LoggedOut { get; set; }
  }
}
