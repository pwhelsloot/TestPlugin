using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server
{
  internal class JobHandlerPlatformCredentials
  {
    [JsonProperty(PlatformClaimType.UserName)]
    public string UserName { get; set; }

    [JsonProperty(PlatformClaimType.TenantId)]
    public string TenantId { get; set; }

    [JsonProperty(PlatformClaimType.Language)]
    public string Language { get; set; }
  }
}