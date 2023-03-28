using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using log4net;

namespace AMCS.Data.Server
{
  public class SecurityTokenManager : ISecurityTokenManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SecurityTokenManager));

    private readonly SecurityTokenConfiguration configuration;
    private readonly TokenValidationParameters tokenValidationParameters;
    private readonly JwtSecurityTokenHandler handler;
    private readonly SigningCredentials signingCredentials;

    public SecurityTokenManager(SecurityTokenConfiguration configuration)
    {
      this.configuration = configuration;

      SecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.EncryptionKey));
      signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);

      tokenValidationParameters = new TokenValidationParameters
      {
        ValidAudience = configuration.Audience,
        ValidateAudience = !string.IsNullOrEmpty(configuration.Audience),
        ValidIssuer = configuration.Issuer,
        ValidateIssuer = !string.IsNullOrEmpty(configuration.Issuer),
        IssuerSigningKey = signingKey
      };

      handler = new JwtSecurityTokenHandler();
    }

    public string WriteToken(IEnumerable<Claim> claims, TimeSpan expiration)
    {
      var token = handler.CreateJwtSecurityToken(
        tokenValidationParameters.ValidIssuer,
        tokenValidationParameters.ValidAudience,
        new ClaimsIdentity(claims),
        DateTime.Now - TimeSpan.FromMinutes(30), // catch issues with clocks being out of sync
        DateTime.Now + expiration,
        DateTime.Now,
        signingCredentials
      );

      return handler.WriteToken(token);
    }

    public JwtSecurityToken ReadToken(string token)
    {
      try
      {
        handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        return (JwtSecurityToken)validatedToken;
      }
      catch (SecurityTokenException securityTokenException)
      {
        Logger.Debug(securityTokenException);
        return null;
      }
    }

  }
}
