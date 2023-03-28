using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public class SQLCriteriaException : Exception
  {
    public SQLCriteriaException()
    {
    }

    public SQLCriteriaException(string message)
      : base(message)
    {
    }

    public SQLCriteriaException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
