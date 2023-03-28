using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetTable
  {
    public DataSet DataSet { get; }

    public IList<DataSetColumn> Columns { get; }

    public IList<IDataSetRecord> Records { get; } = new List<IDataSetRecord>();

    public DataSetTable(DataSet dataSet, IList<DataSetColumn> columns)
    {
      DataSet = dataSet;
      Columns = columns;
    }
  }
}
