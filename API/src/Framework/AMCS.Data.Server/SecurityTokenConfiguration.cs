using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public class SecurityTokenConfiguration
  {
    public string Issuer { get; }
    public string Audience { get; }
    public string EncryptionKey { get; }

    public SecurityTokenConfiguration(string issuer, string audience, string encryptionKey)
    {
      Issuer = issuer;
      Audience = audience;
      EncryptionKey = encryptionKey;
    }
  }
}
