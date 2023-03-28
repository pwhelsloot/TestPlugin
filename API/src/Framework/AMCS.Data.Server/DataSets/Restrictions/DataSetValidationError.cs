using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetValidationError
  {
    public DataSetRestriction Restriction { get; }

    public string Message { get; }

    public DataSetValidationError(DataSetRestriction restriction, string message)
    {
      Restriction = restriction;
      Message = message;
    }
  }
}
