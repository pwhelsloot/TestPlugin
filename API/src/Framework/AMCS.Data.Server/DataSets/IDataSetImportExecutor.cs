using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.Import;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetImportExecutor
  {
    int? SaveRecord(ISessionToken userId, int? id, IDataSetRecord source, DataSetTable table, IList<DataSetColumn> columns, IDataSetDefaultImportIdMapper idMapper, MessageCollection messages, IDataSession dataSession);
    
    IDataSetRecord CreateRecord(Type type);

    IDataSetRecord GetRecord(ISessionToken userId, int id, DataSet dataSet, IDataSession dataSession);

    int? SaveRecord(ISessionToken userId, IDataSetRecord record, DataSet dataSet, IDataSession dataSession);
  }
}
