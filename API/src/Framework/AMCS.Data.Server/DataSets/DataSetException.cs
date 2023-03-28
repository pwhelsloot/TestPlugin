using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetException : Exception
  {
    public DataSetException()
    {
    }

    public DataSetException(string message)
      : base(message)
    {
    }

    public DataSetException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
