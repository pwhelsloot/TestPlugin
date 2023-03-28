namespace AMCS.Data
{
  using System;
  
  public class StrictModeException : Exception
  {
    public StrictModeException()
    {
    }

    public StrictModeException(string message)
      : base(message)
    {
    }

    public StrictModeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
