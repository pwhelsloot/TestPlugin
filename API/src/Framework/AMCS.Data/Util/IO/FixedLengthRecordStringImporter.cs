namespace AMCS.Data.Util.IO
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public abstract class FixedLengthRecordStringImporter<T>: IRecordStringImporter<T>
  {
    #region Properties/Variables

    protected int[] TokenLengths { get; private set; }

    #endregion Properties/Variables

    #region ctors/dtors

    public FixedLengthRecordStringImporter(int[] tokenLengths)
    {
      TokenLengths = tokenLengths;
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
        for (int i = 0; i < TokenLengths.Length; i++)
        {
          /*if (record.Length < TokenLengths[i])
            throw new Exception("Invalid import record, expected token " + i.ToString() + " to be of length " +
              TokenLengths[i].ToString() + " but record is not long enough");
          */
          string token;

          if (TokenLengths.Length == 1)
          {
            token = record;
          }
          else
          {
            token = record.Substring(0, TokenLengths[i]);
            record = record.Substring(TokenLengths[i]).Trim();
          }

          recordFields.Add(token);
        }
        //if (record.Trim() != string.Empty)
        //  throw new Exception("Invalid import record, surpurflous data of " + record + " detected");
      }
      else
        throw new EmptyRecordStringException("Invalid import record.  It is empty");
      
      return recordFields.ToArray();
    }

    #endregion Methods
  }
}
