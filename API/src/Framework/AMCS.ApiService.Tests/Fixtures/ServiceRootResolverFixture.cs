using AMCS.Data.Configuration;
using Moq;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class ServiceRootResolverFixture : BaseFixture<IServiceRootResolver>
  {
    public ServiceRootResolverFixture SetupGetServiceRoot(string root)
    {
      Mock.Setup(p => p.GetServiceRoot(It.IsAny<string>()))
        .Returns(root);
      return this;
    }
  }
}
