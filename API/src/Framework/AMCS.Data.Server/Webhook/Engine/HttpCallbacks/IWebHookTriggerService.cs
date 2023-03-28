namespace AMCS.Data.Server.Webhook.Engine.HttpCallbacks
{
  using System.Net.Http;
  using System.Threading.Tasks;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server.WebHook.BslTrigger;

  internal interface IWebHookTriggerService
  {
    Task<HttpResponseMessage> Execute(WebHookEntity entity, WebHookConfiguration configuration);
  }
}