﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation
{
  internal class ParseException : Exception
  {
    public ParseException()
    {
    }

    public ParseException(string message)
      : base(message)
    {
    }

    public ParseException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
