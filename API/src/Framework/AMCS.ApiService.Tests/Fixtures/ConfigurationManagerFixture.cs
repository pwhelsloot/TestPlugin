using System.Threading;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class ConfigurationManagerFixture : BaseFixture<IConfigurationManager<OpenIdConnectConfiguration>>
  {
    public ConfigurationManagerFixture SetupGetConfigurationAsync(OpenIdConnectConfiguration configuration = null)
    {
      Mock.Setup(p => p.GetConfigurationAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(configuration ?? new OpenIdConnectConfiguration());
      return this;
    }
  }
}
