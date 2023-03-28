using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class OwinContextFixture : BaseFixture<IOwinContext>
  {
    public OwinContextFixture SetupClaimsUser(ClaimsPrincipal principal)
    {
      var authentication = new Mock<IAuthenticationManager>();
      authentication.Setup(p => p.User).Returns(principal);
      Mock.Setup(p => p.Authentication).Returns(authentication.Object);
      return this;
    }

    public OwinContextFixture SetupNoClaimsUser()
    {
      var authentication = new Mock<IAuthenticationManager>();
      authentication.Setup(p => p.User).Returns(null as ClaimsPrincipal);
      Mock.Setup(p => p.Authentication).Returns(authentication.Object);
      return this;
    }

    public OwinContextFixture SetupRequestWithEmptyHeaders()
    {
      var requestMock = new Mock<IOwinRequest>();
      requestMock.Setup(p => p.Headers)
        .Returns(Moq.Mock.Of<IHeaderDictionary>());

      Mock.Setup(p => p.Request)
        .Returns(requestMock.Object);
      return this;
    }
  }
}
