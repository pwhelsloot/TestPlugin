using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.JobSystem;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetImportJobProgress : IDataSetImportProgress
  {
    private readonly IJobContext context;

    public CancellationToken CancellationToken => context.CancellationToken;

    public DataSetImportJobProgress(IJobContext context)
    {
      this.context = context;
    }

    public void SetProgress(double progress, string status)
    {
      context.SetProgress(progress, status);
    }
  }
}
