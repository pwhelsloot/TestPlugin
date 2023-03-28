using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public class DateConversionException : Exception
  {
    public DateConversionException()
    {
    }

    public DateConversionException(string message)
      : base(message)
    {
    }

    public DateConversionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
