namespace AMCS.Data.Server.Http
{
  using System;
  using System.Net;
  using System.Net.Http;
  using Entity.Tenants;
  using PlatformCredentials;
  using Plugin;
  using Services;

  internal static class HttpCredentialsUtil
  {
    internal static HttpClientHandler CreatePlatformCredentialsClientClientHandler(string tenantId)
    {
      var tenant = DataServices
        .Resolve<ITenantManager>()
        .GetTenant(tenantId);

      return CreatePlatformCredentialsClientClientHandler(tenant);
    }
    
    internal static HttpClientHandler CreatePlatformCredentialsClientClientHandler(ITenant tenant)
    {
      var cookieContainer = new CookieContainer();

      var baseAddress = new Uri(tenant.CoreAppServiceRoot);
      var platformCredentials =
        new PlatformCredentials(PluginHelper.GetCoreAppCredentials(), tenant.TenantId);
      var token = DataServices.Resolve<IPlatformCredentialsTokenManager>().Serialize(platformCredentials);

      cookieContainer.Add(baseAddress, new Cookie(PlatformCredentials.CookieName, token));

      var handler = new HttpClientHandler();
      handler.CookieContainer = cookieContainer;
      return handler;
    }
  }
}