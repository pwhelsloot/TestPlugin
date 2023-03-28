namespace AMCS.Data.Server.Plugin.UpdateNotification
{
  using Autofac;
  using Data.Configuration;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureSingleTenantPluginUpdateService(this DataConfiguration self)
    {
      self.ConfigureUpdateService();

      self.ContainerBuilder
        .RegisterType<SingleTenantPluginUpdateDiagnosticService>()
        .AsSelf()
        .SingleInstance()
        .AutoActivate()
        .As<IPluginUpdateDiagnosticService>();
    }
    
    public static void ConfigureMultiTenantPluginUpdateService(this DataConfiguration self)
    {
      self.ConfigureUpdateService();
      
      self.ContainerBuilder
        .RegisterType<MultiTenantPluginUpdateDiagnosticService>()
        .AsSelf()
        .SingleInstance()
        .AutoActivate()
        .As<IPluginUpdateDiagnosticService>();
    }
    
    private static void ConfigureUpdateService(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<PluginUpdateNotificationService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IPluginUpdateNotificationService>();
    }
  }
}