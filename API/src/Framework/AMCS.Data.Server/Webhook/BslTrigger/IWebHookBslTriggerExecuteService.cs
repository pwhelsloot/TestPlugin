namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using BslTriggers;

  internal interface IWebHookBslTriggerExecuteService
  {
    void Execute(ISessionToken userId, BslTriggerRequest request, IBslActionContext bslActionContext);
  }
}