namespace AMCS.Data.Util.IO
{
  using System;
  using System.Collections.Generic;
  using System.IO;

  public class RecordFileImporter<T>: IRecordFileImporter<T>
  {
    #region Properties/Variables

    protected IRecordStringImporter<T> RecordStringImporter { get; private set; }

    public IList<T> Records { get; protected set; }

    #endregion Properties/Variables

    #region ctors/dtors

    public RecordFileImporter(IRecordStringImporter<T> recordStringImporter)
    {
      Records = new List<T>();
      RecordStringImporter = recordStringImporter;
    }

    #endregion ctors/dtors

    #region Implemented Methods

    public IList<T> Import(string[] recordFileLines, ref IList<string> exceptions)
    {
      int currentLine = 0;

      foreach (string line in recordFileLines)
      {
        try
        {
            if (!string.IsNullOrEmpty(line))
            {
                currentLine++;
                T record = RecordStringImporter.ImportRecord(line);
                Records.Add(record);
            }
        }
        catch (Exception ex)
        {
          if (exceptions != null)
            exceptions.Add(ex.Message + "(" + Convert.ToString(currentLine) + ")");
        }
      }
      return Records;
    }

    public IList<T> Import(string filename, ref IList<string> exceptions)
    {
      string[] lines = File.ReadAllLines(filename);
      return Import(lines, ref exceptions);
    }

    public IList<T> Import(string filename)
    {
      IList<string> exceptions = null;
      return Import(filename, ref exceptions);
    }

    #endregion Implemented Methods
  }
}
