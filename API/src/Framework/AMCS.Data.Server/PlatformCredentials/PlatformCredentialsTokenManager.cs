using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.PlatformCredentials
{
  public class PlatformCredentialsTokenManager : IPlatformCredentialsTokenManager
  {
    private readonly IPlatformCredentialsSecurityTokenManager manager;
    private readonly TimeSpan expiration;

    public PlatformCredentialsTokenManager(IPlatformCredentialsSecurityTokenManager manager, TimeSpan expiration)
    {
      this.manager = manager;
      this.expiration = expiration;
    }

    public PlatformCredentials Deserialize(string token)
    {
      if (!string.IsNullOrEmpty(token))
      {
        var securityToken = manager.ReadToken(token);
        if (securityToken == null)
          return null;

        string userIdentity = null;
        string tenantId = null;
        var roles = Array.Empty<string>();

        foreach (var claim in securityToken.Claims)
        {
          switch (claim.Type)
          {
            case PlatformClaimType.Email:
              userIdentity = claim.Value;
              break;
            case PlatformClaimType.TenantId:
              tenantId = claim.Value;
              break;
            case PlatformClaimType.Roles:
              if (!string.IsNullOrWhiteSpace(claim.Value))
                roles = claim.Value.Split(',');
              break;
          }
        }

        var credentials = new PlatformCredentials(userIdentity, tenantId);
        credentials.Roles.AddRange(roles);

        return credentials;
      }

      return null;
    }

    public string Serialize(PlatformCredentials platformCredentials)
    {
      return manager.WriteToken(
        new[]
        {
          new Claim(PlatformClaimType.Email, platformCredentials.UserIdentity),
          new Claim(PlatformClaimType.TenantId, platformCredentials.TenantId),
          new Claim(PlatformClaimType.Roles, string.Join(",", platformCredentials.Roles))
        }, 
        expiration);
    }
  }
}
