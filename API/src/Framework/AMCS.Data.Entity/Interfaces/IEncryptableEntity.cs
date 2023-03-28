using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Interfaces
{
  public interface IEncryptableEntity
  {
    Dictionary<string, bool> EncryptedProperties { get; }

    bool IsEncrypted { get; set; }
  }
}