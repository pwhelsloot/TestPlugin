namespace AMCS.Data.Server
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Security.Claims;
  using Services;

  public class SessionTokenManager : ISessionTokenManager
  {
    private readonly ISecurityTokenManager manager;
    private readonly TimeSpan expiration;

    public SessionTokenManager(ISecurityTokenManager manager, TimeSpan expiration)
    {
      this.manager = manager;
      this.expiration = expiration;
    }

    public SessionTokenStatus TryDeserialize(string token, out ISessionToken sessionToken)
    {
      if (string.IsNullOrEmpty(token))
        throw new ArgumentException("Token cannot be empty", nameof(token));

      sessionToken = null;

      var securityToken = manager.ReadToken(token);

      if (securityToken == null)
        return SessionTokenStatus.CannotDecrypt;

      string userName = null;
      string userIdentity = null;
      int userId = 0;
      int companyOutletId = 0;
      bool isOfflineModeEnabled = false;
      string tenantId = null;
      CultureInfo language = null;

      foreach (var claim in securityToken.Claims)
      {
        switch (claim.Type)
        {
          case PlatformClaimType.UserName:
            userName = claim.Value;
            break;
          case PlatformClaimType.Email:
            userIdentity = claim.Value;
            break;
          case PlatformClaimType.UserId:
            userId = int.Parse(claim.Value, CultureInfo.InvariantCulture);
            break;
          case PlatformClaimType.CompanyOutletId:
            companyOutletId = int.Parse(claim.Value, CultureInfo.InvariantCulture);
            break;
          case PlatformClaimType.IsOfflineModeEnabled:
            isOfflineModeEnabled = int.Parse(claim.Value, CultureInfo.InvariantCulture) != 0;
            break;
          case PlatformClaimType.TenantId:
            tenantId = claim.Value;
            break;
          case PlatformClaimType.Language:
            language = CultureInfo.CurrentUICulture;
            break;
        }
      }

      sessionToken = DataServices.Resolve<IUserService>().CreateSessionToken(new AuthenticationUser(
        userName,
        userIdentity,
        userId,
        null,
        companyOutletId,
        null,
        isOfflineModeEnabled,
        tenantId,
        language));

      return SessionTokenStatus.Valid;
    }

    public string Serialize(ISessionToken token)
    {
      if (token == null)
        throw new ArgumentNullException(nameof(token));

      var claims = new List<Claim>
      {
        new Claim(PlatformClaimType.UserName, token.UserName),
        new Claim(PlatformClaimType.UserId, token.UserId.ToString(CultureInfo.InvariantCulture)),
        new Claim(PlatformClaimType.CompanyOutletId, token.CompanyOutletId.ToString(CultureInfo.InvariantCulture))
      };

      if (token.IsOfflineModeEnabled)
        claims.Add(new Claim(PlatformClaimType.IsOfflineModeEnabled, "1"));
      if (token.TenantId != null)
        claims.Add(new Claim(PlatformClaimType.TenantId, token.TenantId));
      if (token.UserIdentity != null)
        claims.Add(new Claim(PlatformClaimType.Email, token.UserIdentity)); 
      if (token.Language != null)
        claims.Add(new Claim(PlatformClaimType.Language, token.Language.Name));

      return manager.WriteToken(claims, expiration);
    }
  }
}
