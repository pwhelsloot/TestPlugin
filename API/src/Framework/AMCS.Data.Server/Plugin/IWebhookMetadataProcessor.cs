namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  using AMCS.PluginData.Data.Metadata.WebHooks;

  public interface IWebHookMetadataProcessor
  {
    void Process(IList<WebHook> webHooks, string fullyQualifiedPluginName,
      ISessionToken sessionToken, IDataSession dataSession);
  }
}