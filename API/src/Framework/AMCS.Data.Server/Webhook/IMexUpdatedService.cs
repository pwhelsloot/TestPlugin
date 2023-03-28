using System;

namespace AMCS.Data.Server.WebHook
{
  public interface IMexUpdatedService
  {
    event WebHookCallbackEventHandler MexUpdated;
  }
}