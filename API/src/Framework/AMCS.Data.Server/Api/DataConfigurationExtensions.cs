namespace AMCS.Data.Server.Api
{
  using AMCS.Data.Configuration;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureRestApiService(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<RestApiService>()
        .SingleInstance()
        .As<IRestApiService>();
    }
  }
}