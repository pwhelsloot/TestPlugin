using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal interface IImportQueueRequester
  {
    ImportBatch GetNextBatch();

    void ReportErrors(int errors);
  }
}
