namespace AMCS.ApiService.Documentation.NETCore.Configuration
{
  using AMCS.ApiService.Abstractions.MvcSetup;
  using AMCS.ApiService.Documentation.Abstractions.Swagger;
  using AMCS.ApiService.Documentation.NETCore.Swagger;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Services;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureApiExplorer(this DataConfiguration self, ApiDocumentationConfiguration configuration)
    {
      self.ContainerBuilder
        .Register(p => new ApiExplorerConfiguration(p.Resolve<IMvcSetupService>(),
          p.Resolve<IAppSetupService>(),
          configuration,
          p.Resolve<IServiceRootResolver>()))
        .SingleInstance()
        .AutoActivate()
        .As<IApiExplorerConfiguration>()
        .AsSelf();
    }
  }
}