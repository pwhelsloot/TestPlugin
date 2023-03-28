using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AMCS.Data.Indexing
{
  public class BasicRangeIndex : IRangeIndex
  {
    #region IRangeIndex Members

    public string Name { get; set; }

    public long Offset { get; set; }

    public long Length { get; set; }

    #endregion
  }
}
