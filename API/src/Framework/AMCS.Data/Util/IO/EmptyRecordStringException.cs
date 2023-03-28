namespace AMCS.Data.Util.IO
{
  using System;

  public class EmptyRecordStringException : InvalidRecordStringException
  {
    public EmptyRecordStringException(string message) : base(message) { }
  }
}
