namespace AMCS.Data.Server.Http
{
  using AMCS.Data.Configuration;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureHttpServices(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<HttpRetryService>()
        .SingleInstance()
        .As<IHttpRetryService>();

      self.ContainerBuilder
        .RegisterType<HttpRetryAttemptTime>()
        .SingleInstance()
        .As<IHttpRetryAttemptTime>();

      self.ContainerBuilder
        .RegisterType<AmcsHttpClientFactory>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IAmcsHttpClientFactory>();
    }
  }
}