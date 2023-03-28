using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetImportJob
  {
    JobStatus Status { get; }

    TimeSpan Runtime { get; }

    double? LastProgress { get; }

    string LastStatus { get; }

    string Result { get; }
  }
}
