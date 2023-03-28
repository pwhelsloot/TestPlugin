/* TODO: Disabled because of issues with System.Net.Http.

using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Configuration;
using AMCS.ApiService.Tests.Fixtures;
using AMCS.Data.Mocking;
using AMCS.Data.Server.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AMCS.ApiService.Tests.Configuration
{
  [TestClass]
  public class OwinContextExtensionsTests
  {
    private const string Token = "access-token.unit-test";

    private const string ClientId = "unit-test-client-id";

    private const string ClientSecret = "unit-test-client-secret";

    private const string GoogleRevokeEndpoint = "https://oauth2.googleapis.com/revoke";

    private const string GoogleIssuer = "https://accounts.google.com";

    private const string OktaRevokeEndpoint = "https://dev-123456.okta.com/revoke";

    private const string OktaIssuer = "https://dev-123456.okta.com/oauth2/default";

    private ISsoConfiguration Configuration => new SsoConfigurationFixture()
      .SetupOpenConnect(new SsoOpenIdConnectConfigurationFixture()
        .SetupClientId(ClientId)
        .SetupSecret(ClientSecret)
        .Create())
      .Create();

    [DataTestMethod]
    [DataRow(GoogleRevokeEndpoint, GoogleIssuer)]
    [DataRow(OktaRevokeEndpoint, OktaIssuer)]
    public async Task WhenSsoAuthenticated_OpenIdConfigurationPresent_AccessTokenIsRevoked(string revocationEndpoint, string issuer)
    {
      var context = new OwinContextFixture()
        .SetupClaimsUser(new ClaimsPrincipalFixture()
          .AddClaims(new Claim("iss", issuer), GetAccessTokenClaim(Token))
          .Create())
        .Create();

      HttpRequestMessage request = null;
      var handlerFixture = new HttpMessageHandlerFixture()
        .SetupOkPost("/revoke", "{}", r => request = r);

      var options = new OpenIdConnectAuthenticationOptionsFixture()
        .SetupConfiguration(new OpenIdConnectConfiguration() { AdditionalData = { { "revocation_endpoint", revocationEndpoint } } })
        .Create();

      var redirectNotification =
        new RedirectToIdentityProviderNotificationFixture<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>()
          .SetupOptions(options)
          .Create();

      var mockedServices = new MockDataServices()
        .Add(Configuration);

      using (mockedServices.Activate())
      {
        await context.RevokeOAuth2TokenAsync(redirectNotification, new HttpClient(handlerFixture.Create()));
      }

      Assert.AreEqual(NotificationResultState.HandledResponse, redirectNotification.State);
      Assert.AreEqual("/revoke", request.RequestUri.AbsolutePath);
      Assert.AreEqual($"?token={Token}", request.RequestUri.Query);
      Assert.AreEqual("Basic", request.Headers.Authorization.Scheme);
      Assert.AreEqual(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}")), request.Headers.Authorization.Parameter);
      Assert.AreEqual("application/x-www-form-urlencoded", request.Content.Headers.ContentType.MediaType);
      handlerFixture.Mock.Verify(p => p.TestSendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow(GoogleRevokeEndpoint, GoogleIssuer)]
    [DataRow(OktaRevokeEndpoint, OktaIssuer)]
    public async Task WhenSsoAuthenticated_OpenIdConfigurationNotPresent_AccessTokenIsRevoked(string revocationEndpoint, string issuer)
    {
      var context = new OwinContextFixture()
        .SetupClaimsUser(new ClaimsPrincipalFixture()
          .AddClaims(new Claim("iss", issuer), GetAccessTokenClaim(Token))
          .Create())
        .Create();

      HttpRequestMessage request = null;
      var handlerFixture = new HttpMessageHandlerFixture()
        .SetupOkPost("/revoke", "{}", r => request = r);

      var options = new OpenIdConnectAuthenticationOptionsFixture()
        .SetupConfigurationManager(new ConfigurationManagerFixture()
          .SetupGetConfigurationAsync(new OpenIdConnectConfiguration() { AdditionalData = { { "revocation_endpoint", revocationEndpoint } } })
          .Create())
        .Create();

      var redirectNotification =
        new RedirectToIdentityProviderNotificationFixture<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>()
          .SetupOptions(options)
          .Create();

      var mockedServices = new MockDataServices()
        .Add(Configuration);

      using (mockedServices.Activate())
      {
        await context.RevokeOAuth2TokenAsync(redirectNotification, new HttpClient(handlerFixture.Create()));
      }

      Assert.AreEqual(NotificationResultState.HandledResponse, redirectNotification.State);
      Assert.AreEqual("/revoke", request.RequestUri.AbsolutePath);
      Assert.AreEqual($"?token={Token}", request.RequestUri.Query);
      Assert.AreEqual("Basic", request.Headers.Authorization.Scheme);
      Assert.AreEqual(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}")), request.Headers.Authorization.Parameter);
      Assert.AreEqual("application/x-www-form-urlencoded", request.Content.Headers.ContentType.MediaType);
      handlerFixture.Mock.Verify(p => p.TestSendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow(GoogleRevokeEndpoint, GoogleIssuer)]
    [DataRow(OktaRevokeEndpoint, OktaIssuer)]
    public async Task WhenSsoAuthenticated_OpenIdConfigurationPresent_RevokeFails_NoExceptionThrown(string revocationEndpoint, string issuer)
    {
      var context = new OwinContextFixture()
        .SetupClaimsUser(new ClaimsPrincipalFixture()
          .AddClaims(new Claim("iss", issuer), GetAccessTokenClaim(Token))
          .Create())
        .Create();

      var handlerFixture = new HttpMessageHandlerFixture()
        .SetupBadRequestPost("/revoke", @"{""error"":""token is revoked already""}");

      var options = new OpenIdConnectAuthenticationOptionsFixture()
        .SetupConfiguration(new OpenIdConnectConfiguration() { AdditionalData = { { "revocation_endpoint", revocationEndpoint } } })
        .Create();

      var redirectNotification =
        new RedirectToIdentityProviderNotificationFixture<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>()
          .SetupOptions(options)
          .Create();

      var mockedServices = new MockDataServices()
        .Add(Configuration);

      using (mockedServices.Activate())
      {
        await context.RevokeOAuth2TokenAsync(redirectNotification, new HttpClient(handlerFixture.Create()));
      }
    }

    [DataTestMethod]
    [DataRow(GoogleIssuer)]
    [DataRow(OktaIssuer)]
    public async Task WhenSsoAuthenticated_NoRevocationEndpoint_NoActionIsTaken(string issuer)
    {
      var context = new OwinContextFixture()
        .SetupClaimsUser(new ClaimsPrincipalFixture()
          .AddClaims(new Claim("iss", issuer))
          .Create())
        .Create();

      var handlerFixture = new HttpMessageHandlerFixture();

      var configuration = new SsoConfigurationFixture()
        .SetupOpenConnect(new SsoOpenIdConnectConfigurationFixture()
          .SetupExternalLogout()
          .Create())
        .Create();

      var options = new OpenIdConnectAuthenticationOptionsFixture()
        .SetupConfiguration()
        .Create();

      var redirectNotification =
        new RedirectToIdentityProviderNotificationFixture<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>()
          .SetupOptions(options)
          .Create();

      var mockedServices = new MockDataServices()
        .Add(configuration);

      using (mockedServices.Activate())
      {
        await context.RevokeOAuth2TokenAsync(redirectNotification, new HttpClient(handlerFixture.Create()));
      }

      handlerFixture.Mock.Verify(p => p.TestSendAsync(It.IsAny<HttpRequestMessage>()), Times.Never);
    }

    private static Claim GetAccessTokenClaim(string token)
      => new Claim("access_token", token);
  }
}

*/