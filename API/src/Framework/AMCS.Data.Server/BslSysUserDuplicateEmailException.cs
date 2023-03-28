using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public class BslSysUserDuplicateEmailException : BslUserException
  {
    public BslSysUserDuplicateEmailException()
      : base()
    {
    }

    public BslSysUserDuplicateEmailException(string message)
      : base(message)
    {
    }

    public BslSysUserDuplicateEmailException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
