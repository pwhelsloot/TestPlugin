namespace AMCS.Data.Server.Services
{
  using System;
  using System.Globalization;

  public class AuthenticationUser : IAuthenticationResult
  {
    public string UserName { get; }

    public string UserIdentity { get; }

    public int UserId { get; }

    public Guid? UserGuid { get; }

    public int CompanyOutletId { get; }

    public Guid? CompanyOutletGuid { get; }

    public bool IsOfflineModeEnabled { get; }

    public string TenantId { get; }

    public CultureInfo Language { get; }

    public AuthenticationUser(string userName, string userIdentity, int userId, Guid? userGuid, int companyOutletId, Guid? companyOutletGuid, bool isOfflineModeEnabled, string tenantId, CultureInfo language)
    {
      UserName = userName;
      UserIdentity = userIdentity;
      UserId = userId;
      UserGuid = userGuid;
      CompanyOutletId = companyOutletId;
      CompanyOutletGuid = companyOutletGuid;
      IsOfflineModeEnabled = isOfflineModeEnabled;
      TenantId = tenantId;
      Language = language;
    }
  }
}
