#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService
{
  public class HttpException : Exception
  {
    private readonly int httpCode;

    public HttpException()
    {
    }

    public HttpException(string message)
      : base(message)
    {
    }

    public HttpException(int code, string message)
      : base(message)
    {
      httpCode = code;
    }

    public HttpException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public HttpException(int code, string message, Exception innerException)
      : base(message, innerException)
    {
      httpCode = code;
    }

    public int GetHttpCode()
    {
      return GetHttpCode(this);
    }

    private int GetHttpCode(Exception exception)
    {
      switch (exception)
      {
        case HttpException httpException:
          if (httpException.httpCode > 0)
            return httpException.httpCode;
          break;

        case UnauthorizedAccessException _:
          return 401;

        case PathTooLongException _:
          return 414;
      }

      if (exception.InnerException != null)
        return GetHttpCode(exception.InnerException);

      return 500;
    }
  }
}

#endif
