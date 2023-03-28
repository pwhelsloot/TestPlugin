namespace AMCS.Data.Server.Webhook.Engine.HttpCallbacks
{
  using System;
  using System.Net.Http;
  using System.Threading.Tasks;
  using AMCS.Data.Entity.WebHook;

  internal interface IHttpCallback
  {
    Task<HttpResponseMessage> TrackCallback(WebHookEntity entity, Func<Task<HttpResponseMessage>> callback);
  }
}