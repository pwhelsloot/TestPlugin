namespace AMCS.ApiService.Documentation.NETFramework.Configuration
{
  using AMCS.ApiService.Abstractions.MvcSetup;
  using AMCS.ApiService.Documentation.Abstractions.Swagger;
  using AMCS.ApiService.Documentation.NETFramework.Swagger;
  using AMCS.Data.Configuration;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureApiExplorer(this DataConfiguration self, ApiDocumentationConfiguration configuration)
    {
      self.ContainerBuilder
        .Register(p => new ApiExplorerConfiguration(p.Resolve<IMvcSetupService>(), configuration, p.Resolve<IServiceRootResolver>()))
        .SingleInstance()
        .AutoActivate()
        .As<IApiExplorerConfiguration>()
        .AsSelf();
    }
  }
}