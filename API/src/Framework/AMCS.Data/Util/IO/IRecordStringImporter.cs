namespace AMCS.Data.Util.IO
{
  using System;

  public interface IRecordStringImporter<T>
  {
    T ImportRecord(string record);
  }
}
