using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  public interface IBslAction
  {
    void Execute(ISessionToken userId, BslTriggerRequest request, IBslActionContext bslActionContext);
  }

  public interface IBslAction<T>
  {
    void Execute(ISessionToken userId, BslTriggerRequest request, T configuration, IBslActionContext bslActionContext);
  }
}
