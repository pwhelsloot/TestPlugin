using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public class SQLLiteralExpanderException : Exception
  {
    public SQLLiteralExpanderException()
    {
    }

    public SQLLiteralExpanderException(string message)
      : base(message)
    {
    }

    public SQLLiteralExpanderException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
