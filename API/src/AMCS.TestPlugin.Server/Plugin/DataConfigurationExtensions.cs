namespace AMCS.TestPlugin.Server.Plugin
{
  using Autofac;
  using AMCS.Data.Configuration;
  using AMCS.TestPlugin.Server.Plugin.ReverseProxyRules;
  using AMCS.Data.Server.Plugin.Core;
  using AMCS.Data.Server.Plugin;
  using AMCS.TestPlugin.Server.Plugin.MetadataRegistry;
  using AMCS.TestPlugin.Server.Plugin.Workflows;

  public static class DataConfigurationExtensions
  {
    public static void ConfigurePlugin(this DataConfiguration configuration, string tenantId)
    {
      configuration.ContainerBuilder
        .RegisterType<PluginReverseProxyRuleMetadataService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate();
    }

    public static void ConfigurePluginMetadata(this DataConfiguration configuration, string tenantId)
    {
      configuration.AddWorkflowActivityRegistry();

      configuration.ContainerBuilder
        .RegisterType<WorkflowActivityMetadataService>()
        .SingleInstance()
        .AutoActivate();

      configuration.ConfigureCoreMetadataRegistries();

      configuration.ContainerBuilder
        .RegisterType<PluginWorkflowMetadataService>()
        .WithParameter(nameof(tenantId), tenantId)
        .SingleInstance()
        .AsSelf()
        .AutoActivate();
    }

  }
}