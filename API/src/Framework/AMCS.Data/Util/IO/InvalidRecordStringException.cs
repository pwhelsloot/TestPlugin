namespace AMCS.Data.Util.IO
{
  using System;

  public class InvalidRecordStringException: Exception
  {
    public InvalidRecordStringException(string message) : base(message) { }
  }
}
