using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.OpenIdConnect;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class OpenIdConnectAuthenticationOptionsFixture : BaseFixture<OpenIdConnectAuthenticationOptions>
  {
    public OpenIdConnectAuthenticationOptionsFixture()
      : base(true)
    {
    }

    public OpenIdConnectAuthenticationOptionsFixture SetupConfiguration(OpenIdConnectConfiguration configuration = null)
    {
      Mock.Object.Configuration = configuration ?? new OpenIdConnectConfiguration();
      return this;
    }

    public OpenIdConnectAuthenticationOptionsFixture SetupConfigurationManager(IConfigurationManager<OpenIdConnectConfiguration> configurationManager)
    {
      Mock.Object.ConfigurationManager = configurationManager;
      return this;
    }
  }
}
