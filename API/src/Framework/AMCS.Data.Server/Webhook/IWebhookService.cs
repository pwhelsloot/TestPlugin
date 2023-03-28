namespace AMCS.Data.Server.WebHook
{
  using System;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.WebHook;
  using System.Collections.Generic;
  using Entity.WebHook;
  using AMCS.PluginData.Data.Configuration;

  public interface IWebHookService
  {
    IList<WebHookEntity> GetWebHooks();

    void SaveWebHooks(IList<WebHookEntity> webHooks, string category, ISessionToken sessionToken,
      IDataSession dataSession);

    Guid SaveWebHook(PluginWebHook webHook, string systemCategory, ISessionToken userId, IDataSession dataSession);

    WebHookEntity MapWebHookToEntity(BaseWebHook webHook, string systemCategory, Guid tenantId);

    bool ChangeDetectedInWebHook(PluginWebHook webHook, string systemCategory, Guid tenantId);

    void DeleteWebHook(string webHookGuid, ISessionToken userId, IDataSession dataSession);

    void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies);

    WebHookRegistrationDto Register(string name, WebHookFormat format, WebHookTrigger trigger, Action<WebHookCallback> callback, string filter);

    WebHookRegistrationDto UnRegister(WebHookRegistrationDto webHookRegistration);

    void ExecuteCallback(WebHookCallback webHookCallback);
  }
}