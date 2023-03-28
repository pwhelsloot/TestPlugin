namespace AMCS.Data.Server.Glossary
{
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Api;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.WebHook;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureGlossary(
      this DataConfiguration self,
      string coreServiceRoot,
      string tenantId)
    {
      self.ContainerBuilder
        .RegisterType<GlossaryCacheService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IGlossaryCacheService>();

      self.ContainerBuilder
        .Register(context => new GlossaryService(
          context.Resolve<IUserService>(),
          context.Resolve<GlossaryCacheService>(),
          context.Resolve<IRestApiService>(),
          context.Resolve<ISetupService>(),
          context.Resolve<IWebHookService>(),
          coreServiceRoot,
          tenantId))
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IGlossaryService>();
    }
  }
}