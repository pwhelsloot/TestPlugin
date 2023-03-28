using System;

namespace AMCS.Data.Server.Api
{
  public class RestApiException : Exception
  {
    public RestApiException()
    {
    }

    public RestApiException(string message)
      : base(message)
    {
    }

    public RestApiException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}