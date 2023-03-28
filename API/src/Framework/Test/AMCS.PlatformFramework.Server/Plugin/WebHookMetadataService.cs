namespace AMCS.PlatformFramework.Server.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Server.WebHook;
  using AMCS.Data.Configuration;
  using AMCS.PluginData.Data.WebHook;

  public class WebHookMetadataService : IWebHookMetadataService
  {
    private readonly IWebHookService webHookService;
    private readonly List<WebHookRegistrationDto> webHookRegistrations = new List<WebHookRegistrationDto>();

    public WebHookMetadataService(ISetupService setupService, IWebHookService webHookService)
    {
      this.webHookService = webHookService;

      setupService.RegisterSetup(RegisterWebHooks, 1000);
    }
    
    public void RegisterWebHooks()
    {
      // Below is an example of how an application would register a web hook.
      // This example registers a web hook in core app against changes to the User Entity
      // Note that to run this, support must be added to the core app to raise web hook callbacks for the User Entity
      // var webHookRegistration = webHookService.Register(
      //  $"{PluginHelper.GetCorePluginFullName()}:User",
      //  WebHookTriggerType.Full,
      //  WebHookTrigger.Update,
      //  webHookCallback => 
      //  {
      //    var sb = new StringBuilder()
      //      .AppendLine($"RegistrationKey: {webHookCallback.RegistrationKey}")
      //      .AppendLine($"Id: {webHookCallback.Id}")
      //      .AppendLine($"Trigger: {webHookCallback.Trigger}")
      //      .AppendLine($"TransactionId: {webHookCallback.TransactionId}")
      //      .AppendLine($"Body: {webHookCallback.Body}");

      //    Console.WriteLine($"PlatformFrameworkWebHook raised {sb}");
      //  });

      // webHookRegistrations.Add(webHookRegistration);
    }

    public void UnRegisterWebHooks()
    {
      foreach (var webHookRegistration in webHookRegistrations)
      {
        webHookRegistration.UnRegister();
      }
    }
  }
}