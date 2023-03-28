using System;
using AMCS.Data.Server.Broadcast;

namespace AMCS.Data.Server.WebHook
{

  public class WebHookCallbackEventArgs : EventArgs
  {
    public WebHookCallback WebHookCallback { get; }

    public WebHookCallbackEventArgs(WebHookCallback webHookCallback)
    {
      WebHookCallback = webHookCallback;
    }
  }

  public delegate void WebHookCallbackEventHandler(WebHookCallbackEventArgs e);
}