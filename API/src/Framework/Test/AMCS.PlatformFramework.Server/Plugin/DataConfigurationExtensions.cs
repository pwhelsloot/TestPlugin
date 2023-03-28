namespace AMCS.PlatformFramework.Server.Plugin
{
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Plugin;
  using MetadataRegistry;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    public static void ConfigurePluginMetadata(this DataConfiguration configuration)
    {
      configuration.AddWorkflowActivityRegistry();
      configuration.AddUiComponentsRegistry();

      configuration.ContainerBuilder
        .RegisterType<WorkflowActivityMetadataService>()
        .SingleInstance()
        .AutoActivate();
      
      configuration.ContainerBuilder
        .RegisterType<WebHookMetadataService>()
        .As<IWebHookMetadataService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();

      // optional per application
      configuration.AddMexUpdatedService();

      configuration.ContainerBuilder
        .RegisterType<MexUpdatedListenerService>()
        .AutoActivate()
        .AsSelf();
      
      configuration.ContainerBuilder
        .RegisterType<MexUpdatedListenerService>()
        .AutoActivate()
        .AsSelf();
    }
  }
}