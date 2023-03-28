using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [Flags]
  public enum ApiExplorerMethods
  {
    None = 0,
    Get = 1 << 0,
    GetBlob = 1 << 1,
    GetAssociations = 1 << 2,
    GetCollection = 1 << 3,
    GetChanges = 1 << 4,
    GetNew = 1 << 5,
    Create = 1 << 6,
    Update = 1 << 7,
    Delete = 1 << 8,
    Perform = 1 << 9,
    Other = 1 << 10,
    AllQuery = Get | GetBlob | GetAssociations | GetCollection | GetChanges | GetNew,
    AllUpdate = Create | Update | Delete,
    All = -1
  }
}
