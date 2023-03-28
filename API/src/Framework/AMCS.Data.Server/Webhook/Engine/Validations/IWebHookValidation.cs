namespace AMCS.Data.Server.Webhook.Engine.Validations
{
  using System.Net.Http;
  using System.Threading.Tasks;
  using Entity.WebHook;

  internal interface IWebHookValidation
  {
    Task Validate((WebHookEntity WebHook, HttpResponseMessage Response)[] objs);
  }
}