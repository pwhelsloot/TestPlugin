using System;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Encryption
{
  public interface IEncryptionProviderFactory
  {
    IEncryptionProvider Create(string key);
  }
}
