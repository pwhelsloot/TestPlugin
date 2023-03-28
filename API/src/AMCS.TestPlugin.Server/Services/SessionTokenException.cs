namespace AMCS.TestPlugin.Server.Services
{
  using System;

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
