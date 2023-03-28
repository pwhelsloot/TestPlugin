#if !NETFRAMEWORK

using AMCS.Data.Server.Services;
using AMCS.Data.Server.WebApi;
using AMCS.WebSockets.AspNetCore;

namespace AMCS.Data.Server.Configuration
{
  internal class JobSystemMapper
  {
    public JobSystemMapper(IAppSetupService appSetup)
    {
      appSetup.RegisterConfigure(p => p.MapWebSocketRoute<StatusMonitorAcceptor>("/ws/jobsystem-messaging"));
    }
  }
}

#endif
