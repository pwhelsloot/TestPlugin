using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  internal class BslActionCollection
  {
    public List<BslPendingTrigger> PendingTriggers { get; }
    public List<BslPendingTrigger> PendingJobSystemTriggers { get; }

    public BslActionCollection()
    {
      PendingTriggers = new List<BslPendingTrigger>();
      PendingJobSystemTriggers = new List<BslPendingTrigger>();
    }

    public void Raise(IDataSession session, ISessionToken userId)
    {
      session.Context.Remove(typeof(BslActionCollection).FullName);

      if (PendingTriggers.Count > 0)
      {
        DataServices.Resolve<IBslTriggerManager>().Raise(userId, PendingTriggers);
      }

      if (PendingJobSystemTriggers.Count > 0)
      {
        DataServices.Resolve<IBslTriggerManager>().RaiseJobSystemJob(userId, PendingJobSystemTriggers);
      }
    }
  }
}
