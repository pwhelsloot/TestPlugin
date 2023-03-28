#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Abstractions.MvcSetup
{
  public interface IMvcSetupService
  {
    void Register(Action<IMvcSetup> callback, int order = 0);
  }
}

#endif
