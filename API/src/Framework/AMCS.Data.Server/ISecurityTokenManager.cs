using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface ISecurityTokenManager
  {
    string WriteToken(IEnumerable<Claim> claims, TimeSpan expiration);

    JwtSecurityToken ReadToken(string token);
  }
}