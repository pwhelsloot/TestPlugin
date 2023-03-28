using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  public class FilterException : Exception
  {
    public FilterException()
    {
    }

    public FilterException(string message)
      : base(message)
    {
    }

    public FilterException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
