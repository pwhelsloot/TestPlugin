namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Plugin;
  using Entity.Tenants;
  using WebHook;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Services;
  using MetadataRegistry;
  using Services;
  using Autofac;
  using Configuration;
  using Core;
  using MetadataRegistry.BusinessObject;
  using MetadataRegistry.UiComponent;
  using MetadataRegistry.WebHook;
  using MetadataRegistry.WorkflowActivity;
  using UpdateNotification;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureSingleTenantPluginSystem<T>(
      this DataConfiguration self, 
      string vendorId, 
      string pluginId,
      string currentVersion) where T : ITenantManager
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .AsSelf()
        .SingleInstance()
        .As<ITenantManager>();
      
      self.ConfigurePluginSystem(vendorId, pluginId, currentVersion);
      self.ConfigureSingleTenantPluginUpdateService();
    }

    public static void ConfigureSingleTenantPluginSystem(
      this DataConfiguration self, 
      string vendorId, 
      string pluginId,
      string currentVersion, 
      string tenantId, 
      string coreServiceUrl)
    {
      self.ContainerBuilder
        .Register(p => new DefaultTenantManager(tenantId, coreServiceUrl))
        .AsSelf()
        .SingleInstance()
        .As<ITenantManager>();
      
      self.ConfigurePluginSystem(vendorId, pluginId, currentVersion);
      self.ConfigureSingleTenantPluginUpdateService();
    }
    
    public static void ConfigureMultiTenantPluginSystem<T>(
      this DataConfiguration self, 
      string vendorId, 
      string pluginId,
      string currentVersion) where T : ITenantManager
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .AsSelf()
        .SingleInstance()
        .As<ITenantManager>();
      
      self.ConfigurePluginSystem(vendorId, pluginId, currentVersion);
      self.ConfigureMultiTenantPluginUpdateService();
    }

    public static void ConfigureMultiTenantPluginSystem(
      this DataConfiguration self, 
      string vendorId, 
      string pluginId,
      string currentVersion, 
      string tenantId, 
      string coreServiceUrl)
    {
      self.ContainerBuilder
        .Register(p => new DefaultTenantManager(tenantId, coreServiceUrl))
        .AsSelf()
        .SingleInstance()
        .As<ITenantManager>();
      
      self.ConfigurePluginSystem(vendorId, pluginId, currentVersion);
      self.ConfigureMultiTenantPluginUpdateService();
    }

    private static void ConfigurePluginSystem(
      this DataConfiguration self, 
      string vendorId, 
      string pluginId,
      string currentVersion)
    {
      if (string.IsNullOrWhiteSpace(vendorId))
        throw new ArgumentNullException(nameof(vendorId));

      if (string.IsNullOrWhiteSpace(pluginId))
        throw new ArgumentNullException(nameof(pluginId));

      if (string.IsNullOrWhiteSpace(currentVersion))
        throw new ArgumentNullException(nameof(currentVersion));
      
      self.ContainerBuilder
        .RegisterInstance(new PluginSystem(vendorId, pluginId, currentVersion))
        .SingleInstance()
        .As<IPluginSystem>();

      self.ContainerBuilder
        .RegisterType<PluginSerializationService>()
        .SingleInstance()
        .As<IPluginSerializationService>();
      
      self.ContainerBuilder
        .RegisterType<MetadataProcessor>()
        .SingleInstance()
        .As<IMetadataProcessor>();
      
      self.ContainerBuilder
        .RegisterType<PluginMetadataService>()
        .SingleInstance()
        .As<IPluginMetadataService>();
      
      self.ContainerBuilder
        .RegisterType<MexPostProcessingService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IMexPostProcessingService>();

      self.ContainerBuilder
        .RegisterType<CorePluginHttpService>()
        .AsSelf()
        .SingleInstance()
        .As<ICorePluginHttpService>();

      self.ContainerBuilder
        .RegisterType<PluginRegistryService>()
        .AsSelf()
        .SingleInstance()
        .As<IPluginRegistryService>();
      
      self.ContainerBuilder
        .RegisterType<SlotSwitchService>()
        .AsSelf()
        .SingleInstance()
        .As<ISlotSwitchService>();
    }
      
    private class DefaultTenantManager : ITenantManager
    {
      private readonly object syncRoot = new object();
      private readonly Dictionary<string, ITenant> tenants = new Dictionary<string, ITenant>();

      public event TenantsChangedEventHandler TenantsChanged;

      public DefaultTenantManager(string tenantId, string coreServiceRootUrl)
      {
        if (string.IsNullOrEmpty(coreServiceRootUrl) || string.IsNullOrEmpty(tenantId))
          throw new ArgumentException("Expecting both Tenant Id and Core Service Root URL");

        lock (syncRoot)
          tenants.Add(tenantId, new Tenant(tenantId, coreServiceRootUrl));
      }

      public ITenant GetTenant(string tenantId)
      {
        lock (syncRoot)
        {
          var tenant = tenants.Values
            .ToList()
            .SingleOrDefault(linqTenant => string.Compare(linqTenant.TenantId, tenantId, StringComparison.OrdinalIgnoreCase) == 0);
          
          if (tenant == null)
            throw new TenantNotFoundException($"Could not find tenant with tenant id {tenantId}");

          return tenant;
        }
      }

      public IList<ITenant> GetAllTenants()
      {
        lock (syncRoot)
          return tenants.Values.ToList();
      }

      public void AddTenant(string tenantId, string coreServiceRootUrl)
      {
        lock (syncRoot)
        {
          tenants[tenantId] = new Tenant(tenantId, coreServiceRootUrl);

          TenantsChanged?.Invoke(new TenantsChangedEventArgs());
        }
      }
    }
    public static void AddMetadataRegistryMetadataProcessor<T>(this DataConfiguration self)
      where T : IMetadataRegistryMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IMetadataRegistryMetadataProcessor>();
    }

    public static void AddUdfMetadataMetadataProcessor<T>(this DataConfiguration self)
      where T : IUdfMetadataMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IUdfMetadataMetadataProcessor>();
    }

    public static void AddReverseProxyRuleMetadataProcessor<T>(this DataConfiguration self)
      where T : IReverseProxyRuleMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IReverseProxyRuleMetadataProcessor>();
    }

    public static void AddWorkCenterIconMetadataProcessor<T>(this DataConfiguration self)
      where T : IWorkCenterIconMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IWorkCenterIconMetadataProcessor>();
    }

    public static void AddWorkflowMetadataProcessor<T>(this DataConfiguration self)
      where T : IWorkflowMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IWorkflowMetadataProcessor>();
    }
    
    public static void AddWorkflowProviderMetadataProcessor<T>(this DataConfiguration self)
      where T : IWorkflowProviderMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IWorkflowProviderMetadataProcessor>();
    }

    public static void AddWebHookMetadataProcessor<T>(this DataConfiguration self)
      where T : IWebHookMetadataProcessor
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IWebHookMetadataProcessor>();
    }
    
    public static void AddWebHookMetadataRegistry<T>(this DataConfiguration self)
      where T : IWebHookMetadataRegistryService
    {
      self.ContainerBuilder
        .RegisterType<WebHookMetadataRegistryStrategy>()
        .Keyed<IMetadataRegistryStrategy>(MetadataRegistryType.WebHooks)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<T>()
        .As<IWebHookMetadataRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    public static void AddWorkflowActivityRegistry(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<WorkflowActivityMetadataRegistryStrategy>()
        .Keyed<IMetadataRegistryStrategy>(MetadataRegistryType.WorkflowActivities)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<WorkflowActivityMetadataRegistryService>()
        .As<IWorkflowActivityMetadataRegistryService>()
        .SingleInstance()
        .AutoActivate();
    }

    public static void AddUiComponentsRegistry<T>(this DataConfiguration self)
      where T : IUiComponentsMetadataRegistryService
    {
      self.ContainerBuilder
        .RegisterType<UiComponentMetadataRegistryStrategy>()
        .Keyed<IMetadataRegistryStrategy>(MetadataRegistryType.UiComponents)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<T>()
        .As<IUiComponentsMetadataRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }
    
    public static void AddUiComponentsRegistry(this DataConfiguration self, Assembly assembly = null)
    {
      self.ContainerBuilder
        .RegisterType<UiComponentMetadataRegistryStrategy>()
        .Keyed<IMetadataRegistryStrategy>(MetadataRegistryType.UiComponents)
        .SingleInstance();

      self.ContainerBuilder
        .Register(context => new UiComponentsMetadataRegistryService(
          context.Resolve<IPluginMetadataService>(),
          context.Resolve<IProjectConfiguration>(),
          context.Resolve<IServiceRootResolver>(),
          context.Resolve<IServerConfiguration>(),
          assembly ?? Assembly.GetEntryAssembly()))
        .As<IUiComponentsMetadataRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    public static void AddBusinessObjectMetadataRegistry<T>(this DataConfiguration self)
      where T : IBusinessObjectMetadataRegistryService
    {
      self.ContainerBuilder
        .RegisterType<BusinessObjectMetadataRegistryStrategy>()
        .Keyed<IMetadataRegistryStrategy>(MetadataRegistryType.BusinessObjects)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<T>()
        .As<IBusinessObjectMetadataRegistryService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    public static void AddMexUpdatedService(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<MexUpdatedService>()
        .As<IMexUpdatedService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }
  }
}
