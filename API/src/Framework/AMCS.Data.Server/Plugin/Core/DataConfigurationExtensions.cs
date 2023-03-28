namespace AMCS.Data.Server.Plugin.Core
{
  using AMCS.Data.Configuration;
  using Autofac;
  
  public static class DataConfigurationExtensions
  {
    public static void ConfigureCoreMetadataRegistries(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<CoreUiComponentRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<ICoreUiComponentRegistryService>();
      
      self.ContainerBuilder
        .RegisterType<CoreWorkflowActivityRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<ICoreWorkflowActivityRegistryService>();
      
      self.ContainerBuilder
        .RegisterType<CoreWebHookRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<ICoreWebHookRegistryService>();
    }
  }
}
