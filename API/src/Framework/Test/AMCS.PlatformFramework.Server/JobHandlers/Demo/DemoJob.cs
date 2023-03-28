using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.JobSystem;

namespace AMCS.PlatformFramework.Server.JobHandlers.Demo
{
  [DisplayName("Demo Job")]
  [Job(AllowScheduling = true)]
  public class DemoJob : JobHandler<DemoRequest, DemoResponse>
  {
    protected override DemoResponse Execute(IJobContext context, ISessionToken userId, DemoRequest request)
    {
      int steps = request.Steps.GetValueOrDefault(5);

      for (int i = 0; i < steps; i++)
      {
        context.SetProgress(i, steps, $"Running step {i + 1}");
        context.Log.Info($"Running step {i + 1}");

        Thread.Sleep(TimeSpan.FromSeconds(1));
      }

      context.SetProgress(1, "Done");
      context.Log.Info("Done");

      return new DemoResponse();
    }
  }
}
