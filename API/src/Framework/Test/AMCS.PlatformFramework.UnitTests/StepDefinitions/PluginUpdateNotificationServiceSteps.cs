using AMCS.Data.Entity.Plugin;
using AMCS.Data.Entity.Tenants;
using AMCS.Data.Server.Http;
using AMCS.Data.Server.PlatformCredentials;
using AMCS.Data.Server.Plugin.UpdateNotification;
using AMCS.Data.Server.Services;
using AMCS.PluginData.Services;
using NUnit.Framework;
using System.Net;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class PluginUpdateNotificationServiceSteps
  {
    private ManualResetEventSlim @event = new ManualResetEventSlim(false);
    private ScenarioContext scenarioContext;
    private const string TENANT_MANAGER_MOCK = "TenantManagerMock";
    private const string HTTP_CLIENT_MOCK = "HttpClientMock";

    public PluginUpdateNotificationServiceSteps(ScenarioContext scenarioContext)
    {
      this.scenarioContext = scenarioContext;
    }

    [Given(@"multiple http clients running")]
    public void GivenMultipleHttpClientsRunning()
    {
      // We run the http calls in a background thread, so use a mre to control flow

      scenarioContext[TENANT_MANAGER_MOCK] = new TenantManagerMock
      {
        ReturnMultiple = true
      };
      scenarioContext[HTTP_CLIENT_MOCK] = new HttpMock(@event, scenarioContext.Get<TenantManagerMock>(TENANT_MANAGER_MOCK).GetAllTenants().Count)
      {
        UsingMultipleTenants = true
      };
    }

    [When(@"plugin update notification service is initiated")]
    public void WhenPluginUpdateNotificationServiceIsInitiated()
    {
      var service = new PluginUpdateNotificationService(
          scenarioContext.Get<TenantManagerMock>(TENANT_MANAGER_MOCK),
          new PlatformCredentialsTokenManagerMock(),
          new PluginSerializationService(),
          new PluginSystemMock(),
          scenarioContext.Get<HttpMock>(HTTP_CLIENT_MOCK));

      service.Start();

      @event.Wait();
    }

    [Then(@"expected results are shown")]
    public void ThenExpectedResultsAreShown()
    {
      // Just making sure we have the expected amount of tenants
      Assert.AreEqual(6, scenarioContext.Get<TenantManagerMock>(TENANT_MANAGER_MOCK).GetAllTenants().Count);

      // We should have called the PutAsync method for each tenant
      Assert.AreEqual(scenarioContext.Get<TenantManagerMock>(TENANT_MANAGER_MOCK).GetAllTenants().Count, scenarioContext.Get<HttpMock>(HTTP_CLIENT_MOCK).FinalAttempts);
    }

    [Given(@"multiple http clients running and (.*) retry attempts")]
    public void GivenMultipleHttpClientsRunningAndRetryAttempts(int retryAttempts)
    {      
      scenarioContext[HTTP_CLIENT_MOCK] = new HttpMock(@event, retryAttempts);
    }

    [When(@"plugin update notification service is initiated and disposed")]
    public void WhenPluginUpdateNotificationServiceIsInitiatedAndDisposed()
    {
      var service = new PluginUpdateNotificationService(
         new TenantManagerMock(),
         new PlatformCredentialsTokenManagerMock(),
         new PluginSerializationService(),
         new PluginSystemMock(),
          scenarioContext.Get<HttpMock>(HTTP_CLIENT_MOCK));

      service.Start();

      scenarioContext.Get<HttpMock>(HTTP_CLIENT_MOCK).Dispose();
      @event.Wait();
    }

    [Then(@"retry is attempted by (.*) retry attempts provided")]
    public void ThenRetryIsAttemptedByRetryAttemptsProvided(int retryAttempts)
    {
      // We should have called the PutAsync method 4 times
      Assert.AreEqual(retryAttempts, scenarioContext.Get<HttpMock>(HTTP_CLIENT_MOCK).FinalAttempts);
    }

    private class HttpMock : IHttpClient, IDisposable
    {
      private readonly ManualResetEventSlim @event;
      private readonly int attempts;

      public bool IsDisposed { get; private set; }

      public bool UsingMultipleTenants { get; set; }

      private int finalAttempts;
      public int FinalAttempts => finalAttempts;

      public HttpMock(ManualResetEventSlim @event, int attempts = 1)
      {
        this.@event = @event;
        this.attempts = attempts;
      }

      public void Dispose()
      {
        IsDisposed = true;
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
      {
        if (UsingMultipleTenants)
        {
          var result = new HttpResponseMessage(HttpStatusCode.NoContent);
          Interlocked.Increment(ref finalAttempts);
          if (attempts == finalAttempts)
            @event.Set();

          return Task.FromResult(result);
        }
        else
        {
          var errorCode = attempts == FinalAttempts
            ? HttpStatusCode.NoContent
            : HttpStatusCode.InternalServerError;

          var result = new HttpResponseMessage(errorCode);

          if (!result.IsSuccessStatusCode)
            Interlocked.Increment(ref finalAttempts);
          else
            @event.Set();

          return Task.FromResult(result);
        }
      }

      public Task<HttpResponseMessage> SendAsyncWithCoreCredentials(HttpRequestMessage requestMessage, ITenant tenant)
      {
        throw new NotImplementedException();
      }
      
      public Task<HttpResponseMessage> SendAsyncWithCoreCredentials(HttpRequestMessage requestMessage, string tenantId)
      {
        throw new NotImplementedException();
      }

      public Task<HttpResponseMessage> GetAsync(string requestUri)
      {
        throw new NotImplementedException();
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CookieContainer cookieContainer)
      {
        throw new NotImplementedException();
      }

      public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
      {
        throw new NotImplementedException();
      }
    }

    private class TenantManagerMock : ITenantManager
    {
      public bool ReturnEmpty { get; set; }

      public bool ReturnMultiple { get; set; }

      public event TenantsChangedEventHandler TenantsChanged;

      public ITenant GetTenant(string tenantId)
      {
        throw new NotImplementedException();
      }

      public IList<ITenant> GetAllTenants()
      {
        if (ReturnMultiple)
          return new List<ITenant>
          {
            new TenantMockEntity("1234", "http://localhost1:26519"),
            new TenantMockEntity("4321", "http://localhost2:26519"),
            new TenantMockEntity("3215", "http://localhost3:26519"),
            new TenantMockEntity("4234", "http://localhost4:26519"),
            new TenantMockEntity("7234", "http://localhost5:26519"),
            new TenantMockEntity("6424", "http://localhost6:26519"),
          };

        return ReturnEmpty
          ? new List<ITenant>()
          : new List<ITenant> { new TenantMockEntity("1234", "http://localhost:26519") };
      }

      public void AddTenant(string tenantId, string coreServiceRootUrl)
      {
        throw new NotImplementedException();
      }
    }

    private class PluginSystemMock : IPluginSystem
    {
      public string VendorId => "AMCS";

      public string PluginId => "MOCKED";

      public string FullyQualifiedName => "AMCS/MOCKED";

      public string CurrentVersion => "1.1.1.1";
    }

    private class PlatformCredentialsTokenManagerMock : IPlatformCredentialsTokenManager
    {
      public PlatformCredentials Deserialize(string token)
      {
        return new PlatformCredentials("app:amcs/core", "1111");
      }

      public string Serialize(PlatformCredentials platformCredentials)
      {
        return "<some token>";
      }
    }
  }
}
