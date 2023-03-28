using System;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Data.Server.PlatformCredentials
{
  public class PlatformCredentials
  {
    public const string CookieName = "Platform.Credentials";

    public string UserIdentity { get; }

    public string TenantId { get; }

    public List<string> Roles { get; } = new List<string>();

    public PlatformCredentials(string userIdentity, string tenantId)
    {
      UserIdentity = userIdentity;
      TenantId = tenantId;
    }
  }
}
