using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Encryption.BouncyCastle
{
  public static class BouncyEncryptionServiceFactory
  {
    public static IList<IEncryptionProviderFactory> Providers = new ReadOnlyCollection<IEncryptionProviderFactory>(
      EncryptionServiceFactory.Providers
        .Concat(new[]
        {
          AESGCMEncryptionProvider.Factory
        })
        .ToList()
    );

    public static IEncryptionService Create(string key)
    {
      return new EncryptionService(Providers.Select(p => p.Create(key)).ToList());
    }
  }
}
