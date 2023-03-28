using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.DataSets.Support
{
  public class Job
  {
    public JobStatus Status { get; }

    public TimeSpan Runtime { get; }

    public string Result { get; }

    public Job(JobStatus status, TimeSpan runtime, string result)
    {
      Status = status;
      Runtime = runtime;
      Result = result;
    }
  }
}
