using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCS.Data.Indexing
{
  public interface IRangeIndex
  {
    /// <summary>
    /// Unique name for index
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Offset to region of interest from start of content
    /// </summary>
    long Offset { get; set; }

    /// <summary>
    /// Length of region of interest
    /// </summary>
    long Length { get; set; }
  }
}
