using System;
using System.Diagnostics.CodeAnalysis;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.Util;
using AMCS.Data.Util.Extension;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.Server.Configuration;
using ICredentials = AMCS.Data.Server.Services.ICredentials;

namespace AMCS.PlatformFramework.Server.Services
{
  using System.Globalization;

  [SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Suppressed for the template project only.")]
  public class UserService : IUserService
  {
    public enum ServiceString
    {
      [StringValue("{0} is not registered.")]
      EmailNotRegistered
    }

    private readonly ISessionTokenManager sessionTokenManager;
    private readonly string tenantId;

    public ISessionToken SystemUserSessionKey { get; } = new SystemSessionToken(null);

    public string ApplicationCode { get; } = "PFW";

    public bool SupportsOfflineMode => false;

    public UserService(ISessionTokenManager sessionTokenManager, IPlatformFrameworkConfiguration platformFrameworkConfiguration)
    {
      this.sessionTokenManager = sessionTokenManager;
      tenantId = platformFrameworkConfiguration.TenantId;
    }

    public IAuthenticationResult Authenticate(ICredentials credentials)
    {
      switch (credentials)
      {
        case IdentityCredentials identityCredentials:
          return AuthenticateByIdentity(identityCredentials.Identity, identityCredentials.Tenant);
        case UserCredentials userCredentials:
          return AuthenticateByCredentials(userCredentials.UserName, userCredentials.Password, true);
        case UserNameCredentials userNameCredentials:
          return AuthenticateByCredentials(userNameCredentials.UserName, null, false);
        case AppCredentials appCredentials:
          return AuthenticateByAppRegistration(appCredentials.App, appCredentials.Tenant);
        default:
          return new AuthenticationFailure(AuthenticationStatus.InvalidCredentials);
      }
    }

    private IAuthenticationResult AuthenticateByAppRegistration(string app, string tenant)
    {
      if (string.Compare(tenant, tenantId, StringComparison.OrdinalIgnoreCase) == 0)
        return new AuthenticationUser(
          app,
          app,
          0,
          null,
          0,
          null,
          false,
          tenant,
          CultureInfo.CurrentUICulture);

      return new AuthenticationFailure(AuthenticationStatus.InvalidCredentials);
    }
    
    private IAuthenticationResult AuthenticateByIdentity(string identity, string tenant)
    {
      IAuthenticationResult result = null;

      if (!string.Equals(tenant, DataServices.Resolve<IPlatformFrameworkConfiguration>().TenantId, StringComparison.OrdinalIgnoreCase))
        return new AuthenticationFailure(AuthenticationStatus.InvalidCredentials);

      if (identity != null)
      {
        using (var session = BslDataSessionFactory.GetDataSession())
        using (var transaction = session.CreateTransaction())
        {
          UserEntity userEntity;

          if (identity.StartsWith("username:", StringComparison.OrdinalIgnoreCase))
            userEntity = DataServices.Resolve<User.IUserService>().GetByName(SystemUserSessionKey, identity, session);
          else
            userEntity = DataServices.Resolve<User.IUserService>().GetByEmailAddress(SystemUserSessionKey, identity, session);
          
          if (userEntity == null)
          {
            throw BslUserExceptionFactory<BslUserException>.CreateException(
              this.GetType(), 
              typeof(ServiceString), 
              (int)ServiceString.EmailNotRegistered, 
              new object[] { identity });
          }

          result = new AuthenticationUser(
            userEntity.UserName,
            userEntity.EmailAddress,
            userEntity.Id32,
            userEntity.GUID,
            0,
            null,
            false,
            tenantId,
            CultureInfo.CurrentUICulture);

          transaction.Commit();
        }
      }

      if (result == null)
        return new AuthenticationFailure(AuthenticationStatus.InvalidCredentials);

      return result;
    }

    private IAuthenticationResult AuthenticateByCredentials(string userName, string password, bool validatePassword)
    {
      IAuthenticationResult result = null;

      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        var userEntity = DataServices.Resolve<User.IUserService>().GetByName(SystemUserSessionKey, userName, session);
        if (userEntity != null)
        {
          bool passwordValid;

          if (validatePassword)
            passwordValid = PasswordHashing.Verify(userName, password, userEntity.Password);
          else
            passwordValid = true;

          if (passwordValid)
          {
            result = new AuthenticationUser(
              userEntity.UserName,
              userEntity.EmailAddress,
              userEntity.Id32,
              userEntity.GUID,
              0,
              null,
              false,
              tenantId,
              CultureInfo.CurrentUICulture);
          }
        }

        transaction.Commit();
      }

      if (result == null)
        return new AuthenticationFailure(AuthenticationStatus.InvalidCredentials);

      return result;
    }

    public bool IsSystemSessionToken(ISessionToken userId)
    {
      return userId is SystemSessionToken;
    }

    public ISessionToken CreateSessionToken(string userName, int userId, int companyOutletId, bool isOfflineModeEnabled)
    {
      return new SessionToken(userName, userId, companyOutletId, isOfflineModeEnabled, CultureInfo.CurrentUICulture);
    }

    private static ISessionToken CreateSessionToken(string userName, string userIdentity, int userId, int companyOutletId, bool isOfflineModeEnabled, string tenantId, CultureInfo language)
    {
      return new SessionToken(userName, userIdentity, userId, companyOutletId, isOfflineModeEnabled, tenantId, language);
    }

    public void Logout(ISessionToken userId)
    {
      // Nothing to do.
    }

    public bool CheckPluginAccess(Guid sysUserId, int? pluginId)
    {
      throw new NotSupportedException();
    }

    public ISessionToken CreateSessionToken(AuthenticationUser user)
    {
      return CreateSessionToken(
        user.UserName,
        user.UserIdentity,
        user.UserId,
        user.CompanyOutletId,
        user.IsOfflineModeEnabled,
        user.TenantId,
        CultureInfo.CurrentUICulture);
    }

    public string SerializeSessionToken(ISessionToken userId)
    {
      if (userId == null)
        return null;
      return sessionTokenManager.Serialize(userId);
    }

    public ISessionToken DeserializeSessionToken(string token)
    {
      ISessionToken sessionToken = null;

      if (!string.IsNullOrEmpty(token))
      {
        switch (sessionTokenManager.TryDeserialize(token, out sessionToken))
        {
          case SessionTokenStatus.Expired:
            throw new SessionTokenException("Session has expired; please login again");
          case SessionTokenStatus.CannotDecrypt:
            throw new SessionTokenException("Session token is not valid for this environment");
        }
      }

      if (sessionToken == null)
        throw new SessionTokenException("Session token is not in a valid format");

      return sessionToken;
    }

    public SessionTokenStatus TryDeserializeSessionToken(string token, out ISessionToken sessionToken)
    {
      sessionToken = null;

      if (!string.IsNullOrEmpty(token))
        return sessionTokenManager.TryDeserialize(token, out sessionToken);

      return SessionTokenStatus.Invalid;
    }

    public ISessionToken CreateSystemSessionToken(string tenantId = null)
    {
      if (tenantId == null)
        return SystemUserSessionKey;
      return new SystemSessionToken(tenantId);
    }

    private class SessionToken : ISessionToken
    {
      public string UserName { get; }

      public string UserIdentity { get; }

      public int UserId { get; }

      public int CompanyOutletId { get; }

      public bool IsOfflineModeEnabled { get; }

      public string TenantId { get; }

      public CultureInfo Language { get; }

      public SessionToken(string userName, int userId, int companyOutletId, bool isOfflineModeEnabled, CultureInfo language)
      {
        UserName = userName;
        UserId = userId;
        CompanyOutletId = companyOutletId;
        IsOfflineModeEnabled = isOfflineModeEnabled;
        Language = language;
      }

      public SessionToken(string userName, string userIdentity, int userId, int companyOutletId, bool isOfflineModeEnabled, string tenantId, CultureInfo language)
        : this(userName, userId, companyOutletId, isOfflineModeEnabled, language)
      {
        UserIdentity = userIdentity;
        TenantId = tenantId;
      }
    }

    private class SystemSessionToken : ISessionToken
    {
      public string UserName => throw new NotSupportedException("System session token does not have a user name");

      public string UserIdentity => throw new NotSupportedException("System session token does not have a user identifier");

      public int UserId => 0;

      public int CompanyOutletId => throw new NotSupportedException("System session token does not have a company outlet");

      public bool IsOfflineModeEnabled => throw new NotSupportedException("System session token does not have offline mode enabled");

      public string TenantId { get; }

      public CultureInfo Language { get; }

      public SystemSessionToken(string tenantId)
      {
        TenantId = tenantId;
        Language = CultureInfo.CurrentUICulture;
      }
    }
  }
}
