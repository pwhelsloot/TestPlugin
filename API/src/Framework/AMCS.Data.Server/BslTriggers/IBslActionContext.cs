using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  public interface IBslActionContext
  {
    IDictionary<string, object> Context { get; }
  }
}
