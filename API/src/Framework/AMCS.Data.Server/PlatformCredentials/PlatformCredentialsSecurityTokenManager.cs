using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using Microsoft.IdentityModel.Tokens;

namespace AMCS.Data.Server.PlatformCredentials
{
  public class PlatformCredentialsSecurityTokenManager : IPlatformCredentialsSecurityTokenManager
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(PlatformCredentialsSecurityTokenManager));

    private readonly PlatformCredentialsSecurityTokenConfiguration configuration;
    private readonly TokenValidationParameters tokenValidationParameters;
    private readonly JwtSecurityTokenHandler handler;
    private readonly SigningCredentials signingCredentials;

    public PlatformCredentialsSecurityTokenManager(PlatformCredentialsSecurityTokenConfiguration configuration)
    {
      this.configuration = configuration;

      SecurityKey signingKey;
      
      var certificateCollection = GetIdentityServerCertificates(configuration);
      if (certificateCollection != null)
      {
        X509Certificate2 certificate = certificateCollection[0];

        foreach (var currentCertificate in certificateCollection)
        {
          if (certificate.NotAfter < currentCertificate.NotAfter)
          {
            certificate = currentCertificate;
          }
        }

        Log.Info($"Using certificate for platform credentials: subject name '{certificate.SubjectName.Name}', thumbprint '{certificate.Thumbprint}'");
        signingCredentials = new X509SigningCredentials(certificate, SecurityAlgorithms.RsaSha512);
        signingKey = signingCredentials.Key;
      }
      else if (!string.IsNullOrEmpty(configuration.EncryptionKey))
      {
        Log.Info("Using private key for platform credentials");
        signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.EncryptionKey));
        signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
      }
      else
      {
        throw new InvalidOperationException("Authentication Certificate Subject Name, Certificate Path, or Encryption Key must be set");
      }

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
        DateTime.Now, // check timezones of these times
        signingCredentials
      );

      return handler.WriteToken(token);
    }

    // create a separate class with handlers and validation parameters
    // list of pairs of handlers and validation parameters for each certificate
    public JwtSecurityToken ReadToken(string token)
    {
      var certificates = GetIdentityServerCertificates(configuration);
      if (certificates != null)
      {
        foreach (var certificate in certificates)
        {
          try
          {
            // can't do this as not thread safe
            tokenValidationParameters.IssuerSigningKey = new X509SigningCredentials(certificate, SecurityAlgorithms.RsaSha512).Key;

            handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            return (JwtSecurityToken)validatedToken;
          }
          catch (SecurityTokenException securityTokenException)
          {
            Log.Error(securityTokenException);
          }
        }
      }

      if (!string.IsNullOrEmpty(configuration.EncryptionKey))
      {
        try
        {
          // write a test to validate token expiration
          handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
          return (JwtSecurityToken)validatedToken;
        }
        catch (SecurityTokenException securityTokenException)
        {
          Log.Debug(securityTokenException);
          return null;
        }
      }

      throw new InvalidOperationException("Authentication Certificate Subject Name, Certificate Path, or Encryption Key must be set");
    }

    private static X509Certificate2Collection GetIdentityServerCertificates(PlatformCredentialsSecurityTokenConfiguration configuration)
    {
      if (!string.IsNullOrEmpty(configuration.CertificatePath))
      {
        var certCollection = new X509Certificate2Collection();
        certCollection.Import(configuration.CertificatePath);

        if (certCollection.Count > 0)
        {
          return certCollection;
        }

        throw new InvalidOperationException($"Authentication certificate(s) could not be found at specified path: { configuration.CertificatePath }.");
      }

      if (!string.IsNullOrEmpty(configuration.CertificateSubjectName))
      {
        using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
        {
          certStore.Open(OpenFlags.ReadOnly);
          var certCollection = certStore.Certificates.Find(
            X509FindType.FindBySubjectName,
            configuration.CertificateSubjectName,
            false);

          if (certCollection.Count > 0)
          {
            return certCollection;
          }

          throw new InvalidOperationException($"Authentication certificate(s) with Subject Name { configuration.CertificateSubjectName } cannot be found in the store.");
        }
      }

      return null;
    }
  }
}
