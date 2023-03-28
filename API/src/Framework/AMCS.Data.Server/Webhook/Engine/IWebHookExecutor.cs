namespace AMCS.Data.Server.Webhook.Engine
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Entity.WebHook;
  using AMCS.Data.Server.WebHook.BslTrigger;

  internal interface IWebHookExecutor
  {
    Task ExecuteAll(List<WebHookEntity> webHooks, WebHookConfiguration configuration);
  }
}