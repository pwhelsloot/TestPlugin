using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Data.Indexing.DelimitedData
{
  public class BasicIndexedDelimitedDataContainer : IIndexedDelimitedDataContainer<BasicRangeIndex>
  {
    #region IIndexedDelimitedDataContainer Members

    public BasicRangeIndex[] Indexes { get; set; }

    #endregion

    #region IDelimitedDataContainer Members

    public string FieldDelimiter { get; set; }

    public string FieldDelimiterEscapeCharacter { get; set; }

    public string RecordDelimiter { get; set; }

    public string RecordDelimiterEscapeCharacter { get; set; }

    public string Data { get; set; }

    #endregion
  }
}
