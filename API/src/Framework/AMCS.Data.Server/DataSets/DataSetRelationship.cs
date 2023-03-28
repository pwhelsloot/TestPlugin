using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetRelationship
  {
    public DataSet From { get; }

    public DataSetColumn FromColumn { get; }

    public DataSet To { get; }

    public DataSetRelationship(DataSet from, DataSetColumn fromColumn, DataSet to)
    {
      From = from;
      FromColumn = fromColumn;
      To = to;
    }
  }
}
