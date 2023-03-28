using System;
using System.Text;
using AMCS.Data.Server.WebHook;

namespace AMCS.PlatformFramework.Server.Plugin
{
  public class MexUpdatedListenerService
  {
    public MexUpdatedListenerService(IMexUpdatedService mexUpdatedService)
    {
      mexUpdatedService.MexUpdated += (e) =>
      {
        // do whatever you want with the web hook
        var sb = new StringBuilder()
          .AppendLine($"RegistrationKey: {e.WebHookCallback.RegistrationKey}")
          .AppendLine($"Id: {e.WebHookCallback.Id}")
          .AppendLine($"Trigger: {e.WebHookCallback.Trigger}")
          .AppendLine($"TransactionId: {e.WebHookCallback.TransactionId}")
          .AppendLine($"Body: {e.WebHookCallback.Body}");

        Console.WriteLine($"Platform Template Plugin Mex Updated Web Hook raised {sb}");
      };
    }
  }
}