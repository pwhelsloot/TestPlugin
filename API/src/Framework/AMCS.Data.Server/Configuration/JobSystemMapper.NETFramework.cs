#if NETFRAMEWORK

using AMCS.Data.Server.Services;
using AMCS.Data.Server.WebApi;
using AMCS.WebSockets.Owin;

namespace AMCS.Data.Server.Configuration
{
  internal class JobSystemMapper
  {
    public JobSystemMapper(IAppSetupService appSetup)
    {
      appSetup.Register(p => p.MapWebSocketRoute<StatusMonitorAcceptor>("/ws/jobsystem-messaging"));
    }
  }
}

#endif
