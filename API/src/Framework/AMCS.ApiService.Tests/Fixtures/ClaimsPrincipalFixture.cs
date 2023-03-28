using System.Linq;
using System.Security.Claims;

namespace AMCS.ApiService.Tests.Fixtures
{
  public class ClaimsPrincipalFixture : BaseFixture<ClaimsPrincipal>
  {
    public ClaimsPrincipalFixture() 
      : base(true)
    {
    }

    public ClaimsPrincipalFixture AddClaims(params Claim[] claims)
    {
      Mock.Object.AddIdentities(claims.Select(p => new ClaimsIdentity(claims)));
      return this;
    }
  }
}
