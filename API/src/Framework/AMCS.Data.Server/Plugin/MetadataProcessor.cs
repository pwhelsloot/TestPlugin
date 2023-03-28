namespace AMCS.Data.Server.Plugin
{
  using System;
  using AMCS.PluginData.Data.Metadata;
  
  public class MetadataProcessor : IMetadataProcessor
  {
    public void ProcessMetadata(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      ProcessReverseProxyRules(metadata, sessionToken, dataSession);
      ProcessWorkCenterIcons(metadata, sessionToken, dataSession);
      ProcessMetadataRegistries(metadata, sessionToken, dataSession);
      ProcessWebHooks(metadata, sessionToken, dataSession);
      ProcessWorkflows(metadata, sessionToken, dataSession);
      ProcessWorkflowProviders(metadata, sessionToken, dataSession);
      ProcessUdfMetadata(metadata, sessionToken, dataSession);
    }

    private static void ProcessReverseProxyRules(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      DataServices.TryResolve<IReverseProxyRuleMetadataProcessor>(out var processor);

      if (metadata.ReverseProxyRules.Count > 0 && processor == null)
        throw new Exception($"{nameof(IReverseProxyRuleMetadataProcessor)} not registered");

      processor?.Process(metadata.ReverseProxyRules, metadata.Plugin, sessionToken, dataSession);
    }
    
    private static void ProcessWorkCenterIcons(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      DataServices.TryResolve<IWorkCenterIconMetadataProcessor>(out var processor);

      if (metadata.WorkCenterIcons.Count > 0 && processor == null)
        throw new Exception($"{nameof(IWorkCenterIconMetadataProcessor)} not registered");

      processor?.Process(metadata.WorkCenterIcons, metadata.Plugin, sessionToken, dataSession);
    }
    
    private static void ProcessMetadataRegistries(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      DataServices.TryResolve<IMetadataRegistryMetadataProcessor>(out var processor);

      if (metadata.MetadataRegistries.Count > 0 && processor == null)
        throw new Exception($"{nameof(IMetadataRegistryMetadataProcessor)} not registered");

      processor?.Process(metadata.MetadataRegistries, metadata.Plugin, sessionToken, dataSession);
    }
    
    private static void ProcessWebHooks(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      DataServices.TryResolve<IWebHookMetadataProcessor>(out var processor);

      if (metadata.WebHooks.Count > 0 && processor == null)
        throw new Exception($"{nameof(IWebHookMetadataProcessor)} not registered");

      processor?.Process(metadata.WebHooks, metadata.Plugin, sessionToken, dataSession);
    }

    private static void ProcessWorkflows(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      DataServices.TryResolve<IWorkflowMetadataProcessor>(out var processor);

      if (metadata.Workflows.Count > 0 && processor == null)
        throw new Exception($"{nameof(IWorkflowMetadataProcessor)} not registered");

      processor?.Process(metadata.Workflows, metadata.Plugin, sessionToken, dataSession);
    }
    
    private static void ProcessWorkflowProviders(
      PluginMetadata metadata, 
      ISessionToken sessionToken,
      IDataSession dataSession)
    {
      DataServices.TryResolve<IWorkflowProviderMetadataProcessor>(out var processor);

      if (metadata.WorkflowProviders.Count > 0 && processor == null)
        throw new Exception($"{nameof(IWorkflowProviderMetadataProcessor)} not registered");

      processor?.Process(metadata, sessionToken, dataSession);
    }

    private static void ProcessUdfMetadata(PluginMetadata metadata, ISessionToken sessionToken, IDataSession dataSession)
    {
      DataServices.TryResolve<IUdfMetadataMetadataProcessor>(out var processor);

      if (metadata.UdfMetadatas.Count > 0 && processor == null)
        throw new Exception($"{nameof(IUdfMetadataMetadataProcessor)} not registered");

      processor?.Process(metadata.UdfMetadatas, metadata.Plugin, sessionToken, dataSession);
    }
  }
}
