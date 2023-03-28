namespace AMCS.Data.Util.IO
{
  using System;
  using System.Collections.Generic;

  public interface IRecordFileImporter<T>
  {
    IList<T> Import(string filename);
  }
}
