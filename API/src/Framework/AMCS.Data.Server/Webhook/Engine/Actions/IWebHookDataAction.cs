namespace AMCS.Data.Server.Webhook.Engine.Actions
{
  using System.Net.Http;
  using System.Threading.Tasks;
  using Entity.WebHook;
  using WebHook.BslTrigger;

  internal interface IWebHookDataAction
  {
    Task Execute((WebHookEntity WebHook, HttpResponseMessage Response)[] data, WebHookConfiguration configuration);
  }
}