using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetImportSinkProgress : IDataSetImportProgress
  {
    public static readonly DataSetImportSinkProgress Instance = new DataSetImportSinkProgress();

    public CancellationToken CancellationToken { get; } = CancellationToken.None;

    private DataSetImportSinkProgress()
    {
    }

    public void SetProgress(double progress, string status)
    {
    }
  }
}
