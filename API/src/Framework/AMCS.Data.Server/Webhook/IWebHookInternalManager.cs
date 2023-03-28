namespace AMCS.Data.Server.Webhook
{
  using System;
  using System.Threading.Tasks;
  using Entity;
  using PluginData.Data.WebHook;
  using WebHook.BslTrigger;

  internal interface IWebHookInternalManager : IWebHookManager
  {
    Task RaiseAsync(WebHookConfiguration configuration, string businessObjectName, bool isValidForCoalesce = true);
  }
}
