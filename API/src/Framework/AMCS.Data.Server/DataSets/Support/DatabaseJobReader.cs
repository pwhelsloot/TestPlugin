using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.SQL;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.DataSets.Support
{
  public class DatabaseJobReader : IJobReader
  {
    public Job GetJobStatus(Guid id)
    {
      Job result;

      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        result = session.Query("select [Status], [Runtime], [Result] from [jobs].[Job] where [GUID] = @ID")
          .Set("ID", id)
          .Execute()
          .SingleOrDefault(p => new Job(
            (JobStatus)p.Get<int>("Status"),
            TimeSpan.FromMilliseconds(p.Get<int?>("Runtime").GetValueOrDefault()),
            p.Get<string>("Result")
          ));

        transaction.Commit();
      }

      return result;
    }
  }
}
