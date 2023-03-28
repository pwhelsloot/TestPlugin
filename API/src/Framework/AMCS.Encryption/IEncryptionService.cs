using System;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Encryption
{
  public interface IEncryptionService
  {
    byte[] Encrypt(byte[] data);

    byte[] Decrypt(byte[] data);
  }
}
