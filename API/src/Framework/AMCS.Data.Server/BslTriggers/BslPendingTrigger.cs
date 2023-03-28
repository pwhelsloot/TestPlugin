using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslPendingTrigger
  {
    [JsonProperty("tid")]
    public int TriggerId { get; }
    [JsonProperty("r")]
    public BslTriggerRequest Request { get; }

    public BslPendingTrigger(int triggerId, BslTriggerRequest request)
    {
      TriggerId = triggerId;
      Request = request;
    }
  }
}
