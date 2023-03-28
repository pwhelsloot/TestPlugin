namespace AMCS.Data.Util.IO
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public abstract class DelimitedRecordStringImporter<T>: IRecordStringImporter<T>
  {
    #region Properties/Variables

    protected char RecordDelimiter { get; private set; }

    #endregion Properties/Variables

    #region ctors/dtors

    public DelimitedRecordStringImporter(char recordDelimiter)
    {
      RecordDelimiter = recordDelimiter;
    }

    #endregion ctors/dtors

    #region Abstract Methods

    public abstract T ImportRecord(string record);

    #endregion Abstract Methods

    #region Methods

    public string[] ConvertRecordToStringArray(string record)
    {
      List<string> recordFields = new List<string>();
      if (!string.IsNullOrEmpty(record))
      {
        string[] tokens = record.Split(RecordDelimiter);
        foreach (string token in tokens)
          recordFields.Add(token.Trim());
      }
      return recordFields.ToArray();
    }

    #endregion Methods
  }
}
