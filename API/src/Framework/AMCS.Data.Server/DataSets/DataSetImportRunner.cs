using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.Import;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetImportRunner
  {
    private readonly ISessionToken userId;
    private readonly DataSetImport import;
    private readonly IDataSetImportProgress progress;

    public DataSetImportRunner(ISessionToken userId, DataSetImport import, IDataSetImportProgress progress)
    {
      this.userId = userId;
      this.import = import;
      this.progress = progress;
    }

    public DataSetImportResult Run()
    {
      var importer = new Importer(userId, import);

      return importer.Import(progress);
    }
  }
}
