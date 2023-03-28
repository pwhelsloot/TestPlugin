using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public enum SessionTokenStatus
  {
    Valid,
    Invalid,
    Expired,
    CannotDecrypt
  }
}
