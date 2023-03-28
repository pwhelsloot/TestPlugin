namespace AMCS.PlatformFramework.Server.Plugin
{
  public interface IWebHookMetadataService
  {
    void RegisterWebHooks();

    void UnRegisterWebHooks();
  }
}