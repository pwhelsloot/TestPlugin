using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class DataSetImportProgressDelegate : IDataSetImportProgress
  {
    private readonly IDataSetImportProgress parent;
    private readonly string statusPrefix;
    private readonly double progressOffset;
    private readonly double progressScale;

    public CancellationToken CancellationToken => parent.CancellationToken;

    public DataSetImportProgressDelegate(IDataSetImportProgress parent, string statusPrefix, double progressOffset, double progressScale)
    {
      this.parent = parent;
      this.statusPrefix = statusPrefix;
      this.progressOffset = progressOffset;
      this.progressScale = progressScale;
    }

    public void SetProgress(double progress, string status)
    {
      parent.SetProgress(progressOffset + (progress * progressScale), statusPrefix + status);
    }
  }
}
