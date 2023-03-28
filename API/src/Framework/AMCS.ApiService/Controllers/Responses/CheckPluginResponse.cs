using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Controllers.Responses
{
  public class CheckPluginResponse
  {
    [JsonProperty("result")]
    public bool Result { get; set; }
  }
}
