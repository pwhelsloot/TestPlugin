namespace AMCS.Data.Server.Services
{
  using System;
  using System.Globalization;

  public interface IUserService
  {
    ISessionToken SystemUserSessionKey { get; }

    string ApplicationCode { get; }

    bool SupportsOfflineMode { get; }

    IAuthenticationResult Authenticate(ICredentials credentials);

    void Logout(ISessionToken userId);

    bool CheckPluginAccess(Guid sysUserId, int? pluginId);

    bool IsSystemSessionToken(ISessionToken userId);

    ISessionToken CreateSessionToken(string userName, int userId, int companyOutletId, bool isOfflineModeEnabled);

    ISessionToken CreateSessionToken(AuthenticationUser user);

    ISessionToken CreateSystemSessionToken(string tenantId = null);

    string SerializeSessionToken(ISessionToken userId);

    ISessionToken DeserializeSessionToken(string token);

    SessionTokenStatus TryDeserializeSessionToken(string token, out ISessionToken sessionToken);
  }
}
