namespace AMCS.Data.Server.WebHook
{
  using Plugin;
  using AMCS.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.WebHook;
  using AMCS.ApiService.Filtering;

  public class MexUpdatedService : IMexUpdatedService
  {
    private readonly IWebHookService webHookService;

    public event WebHookCallbackEventHandler MexUpdated;

    public MexUpdatedService(ISetupService setupService, IWebHookService webHookService)
    {
      this.webHookService = webHookService;

      setupService.RegisterSetup(RegisterWebHook, 1000);
    }

    private void RegisterWebHook()
    {
      webHookService.Register(
        $"{PluginHelper.GetCorePluginFullName()}:MexUpdate",
        WebHookFormat.Simple,
        WebHookTrigger.Insert,
        webHookCallback =>
        {
          MexUpdated?.Invoke(new WebHookCallbackEventArgs(webHookCallback));
        },
        string.Empty
     );
    }
  }
}