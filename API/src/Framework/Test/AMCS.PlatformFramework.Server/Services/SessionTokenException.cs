using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.Server.Services
{
  public class SessionTokenException : Exception
  {
    public SessionTokenException()
    {
    }

    public SessionTokenException(string message)
      : base(message)
    {
    }

    public SessionTokenException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
