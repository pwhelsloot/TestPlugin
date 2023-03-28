using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetDefaultImportIdMapper
  {
    bool TryMapId(DataSet dataSet, int id, IDataSetRecord record, DataSetColumn column, out int result);
  }
}
