namespace AMCS.Data.Server.UserDefinedField
{
  using System;

  internal class UdfValidationException : Exception
  {
    public UdfValidationException()
    {
    }

    public UdfValidationException(string message)
      : base(message)
    {
    }

    public UdfValidationException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}