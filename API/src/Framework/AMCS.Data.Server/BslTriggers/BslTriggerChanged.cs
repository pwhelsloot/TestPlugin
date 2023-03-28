using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslTriggerChanged
  {
    public int TriggerId { get; }

    public BslTriggerChanged(int triggerId)
    {
      this.TriggerId = triggerId;
    }
  }
}
