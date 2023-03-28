using AMCS.Data;
using AMCS.Data.Server;
using NUnit.Framework;
using System;
using System.Threading;
using AMCS.JobSystem;
using AMCS.Data.Server.Services;

namespace AMCS.PlatformFramework.IntegrationTests.SessionToken
{
  using System.Globalization;

  [TestFixture]
  public class SessionTokenFixture : TestBase
  {
    private const string User = "testuser";
    private const string UserIdentityValue = "testuser@amcsgroup.com";
    private const int UserIdValue = 66;
    private const int CompanyOutletIdValue = 666;
    private const bool IsOfflineModeEnabledValue = false;
    private const string TenantIdValue = "1f316784-f00f-4f29-ad2e-8bc2380e0d83";
    private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(30);

    [Test]
    public static void SessionTokenRoundtrip()
    {
      var service = DataServices.Resolve<ISecurityTokenManager>();
      var sessionTokenManager = new SessionTokenManager(service, Expiration);
      var sessionTokenNoTenant = new SessionToken(User, UserIdentityValue, UserIdValue, CompanyOutletIdValue, IsOfflineModeEnabledValue, null, CultureInfo.CurrentUICulture);
      Assert.IsNull(sessionTokenNoTenant.TenantId);
      var encrypted = sessionTokenManager.Serialize(sessionTokenNoTenant);
      sessionTokenManager.TryDeserialize(encrypted, out ISessionToken sessionToken);
      Assert.AreEqual(sessionToken.UserName, User);
      Assert.AreEqual(sessionToken.UserId, UserIdValue);
      Assert.AreEqual(sessionToken.CompanyOutletId, CompanyOutletIdValue);
      Assert.AreEqual(sessionToken.IsOfflineModeEnabled, IsOfflineModeEnabledValue);
    }

    [Test]
    public static void SessionTokenTenantRoundtrip()
    {
      var service = DataServices.Resolve<ISecurityTokenManager>();
      var sessionTokenManager = new SessionTokenManager(service, Expiration);
      var sessionTokenTenant = new SessionToken(User, UserIdentityValue, UserIdValue, CompanyOutletIdValue, IsOfflineModeEnabledValue, TenantIdValue, CultureInfo.CurrentUICulture);
      Assert.IsNotNull(sessionTokenTenant.TenantId);
      var encrypted = sessionTokenManager.Serialize(sessionTokenTenant);
      sessionTokenManager.TryDeserialize(encrypted, out ISessionToken sessionToken);
      Assert.AreEqual(sessionToken.UserName, User);
      Assert.AreEqual(sessionToken.UserId, UserIdValue);
      Assert.AreEqual(sessionToken.CompanyOutletId, CompanyOutletIdValue);
      Assert.AreEqual(sessionToken.IsOfflineModeEnabled, IsOfflineModeEnabledValue);
      Assert.AreEqual(sessionToken.TenantId, TenantIdValue);
    }

    [TestCase("tenantID", "{\"sub\":null,\"sub_tid\":\"tenantID\",\"language\":\"en-GB\"}")]
    [TestCase(null, "{\"sub\":null,\"sub_tid\":null,\"language\":\"en-GB\"}")]
    public static void TenantIdInJobSystem(string tenantId, string expectedJobSystemUser)
    {
      var currentUICulture = CultureInfo.CurrentUICulture;
      CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-GB");

      var systemSessionToken = DataServices.Resolve<IUserService>().CreateSystemSessionToken(tenantId);
      Assert.AreEqual(tenantId, systemSessionToken.TenantId);

      string jobSystemUser = JobHandler.GetJobUserId(systemSessionToken);
      Assert.AreEqual(expectedJobSystemUser, jobSystemUser);

      var job = new FakeJobHandler();
      job.Execute(new FakeJobContext(jobSystemUser), null);

      Assert.AreEqual(tenantId, job.UserId.TenantId);

      CultureInfo.CurrentUICulture = currentUICulture;
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

    private class FakeJobHandler : JobHandler<FakeJobHandler.Request>
    {
      public ISessionToken UserId { get; private set; }

      protected override void Execute(IJobContext context, ISessionToken userId, Request request)
      {
        UserId = userId;
      }

      public class Request
      {
      }
    }

    private class FakeJobContext : IJobContext
    {
      public void SetJobState(string key, string value, bool throwIfNotPersisted = true)
      {
        throw new NotImplementedException();
      }

      public Guid JobId { get; }
      public string UserId { get; }
      public CancellationToken CancellationToken { get; }
      public IJobLog Log { get; }
      public bool IsJobStatePersisted { get; }

      public FakeJobContext(string userId)
      {
        UserId = userId;
      }

      public void SetProgress(double progress, string status)
      {
        throw new NotSupportedException();
      }

      public string GetJobState(string key, bool throwIfNotPersisted = true)
      {
        throw new NotImplementedException();
      }
    }
  }
}
