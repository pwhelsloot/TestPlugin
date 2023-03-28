using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.PlatformCredentials
{
  public class PlatformCredentialsSecurityTokenConfiguration
  {
    public string Issuer { get; }
    public string Audience { get; }
    public string CertificateSubjectName { get; }
    public string CertificatePath { get; }
    public string EncryptionKey { get; }

    public PlatformCredentialsSecurityTokenConfiguration(string issuer, string audience, string certificateSubjectName, string certificatePath, string encryptionKey)
    {
      Issuer = issuer;
      Audience = audience;
      CertificateSubjectName = certificateSubjectName;
      CertificatePath = certificatePath;
      EncryptionKey = encryptionKey;
    }
  }
}
