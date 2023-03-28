namespace AMCS.Data.Server.Webhook.Engine.HttpCallbacks
{
  using System;
  using System.Net.Http;
  using AMCS.Data.Entity.WebHook;

  internal interface IHttpCallbackService
  {
    HttpRequestMessage GenerateRequestMessage(WebHookEntity entity, Uri requestUri);
  }
}
