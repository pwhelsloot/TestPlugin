using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public class SQLRecordNotFoundException : Exception
  {
    public SQLRecordNotFoundException()
    {
    }

    public SQLRecordNotFoundException(string message)
      : base(message)
    {
    }

    public SQLRecordNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
