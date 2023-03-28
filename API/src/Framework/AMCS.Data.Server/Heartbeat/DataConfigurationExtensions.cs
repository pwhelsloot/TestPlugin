namespace AMCS.Data.Server.Heartbeat
{
  using AMCS.Data.Configuration;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void AddHeartbeat(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<HeartbeatService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IHeartbeatService>();
    }
  }
}
