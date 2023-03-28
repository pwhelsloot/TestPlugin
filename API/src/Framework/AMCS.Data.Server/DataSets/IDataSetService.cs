using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetService
  {
    IList<DataSet> DataSets { get; }

    IDataSetsConfiguration Configuration { get; }

    IDataSetImportManager ImportManager { get; }

    DataSet GetDataSet(string name);

    DataSet FindDataSet(string name);

    DataSet GetDataSet(Type type);

    DataSet FindDataSet(Type type);

    DataSetImportResult Import(ISessionToken userId, DataSetImport import, IDataSetImportProgress progress = null);
  }
}
