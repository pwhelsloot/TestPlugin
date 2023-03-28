using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCS.Data.Schema
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface IStoredProcedure: IRoutine
  {
  }
}
