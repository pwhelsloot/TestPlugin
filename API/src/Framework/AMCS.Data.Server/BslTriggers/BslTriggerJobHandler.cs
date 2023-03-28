using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem;

namespace AMCS.Data.Server.BslTriggers
{
  internal class BslTriggerJobHandler : JobHandler<BslTriggerJobRequest>
  {
    protected override void Execute(IJobContext context, ISessionToken userId, BslTriggerJobRequest bslTriggerJobRequest)
    {
      DataServices.Resolve<IBslTriggerManager>().ExecuteBslTriggersFromJobSystem(userId, bslTriggerJobRequest);
    }
  }
}
