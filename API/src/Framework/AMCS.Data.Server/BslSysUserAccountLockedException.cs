﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public class BslSysUserAccountLockedException : BslUserException
  {
    public BslSysUserAccountLockedException()
      : base()
    {
    }

    public BslSysUserAccountLockedException(string message)
      : base(message)
    {
    }

    public BslSysUserAccountLockedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
