using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslTriggerJobRequest
  {
    public IList<BslPendingTrigger> BslPendingTriggers { get; }

    public string BslTriggerJobKey { get; }

    public BslTriggerJobRequest(IList<BslPendingTrigger> bslPendingTriggers, string bslTriggerJobKey)
    {
      BslPendingTriggers = bslPendingTriggers;
      BslTriggerJobKey = bslTriggerJobKey;
    }
  }
}
