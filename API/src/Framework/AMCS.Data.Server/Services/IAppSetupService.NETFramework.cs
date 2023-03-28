#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace AMCS.Data.Server.Services
{
  public interface IAppSetupService
  {
    void Register(Action<IAppBuilder> action, int order = 0);
  }
}

#endif
