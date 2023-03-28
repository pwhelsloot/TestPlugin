namespace AMCS.PlatformFramework.Server.Settings
{
  using AMCS.Data.Server.Services;

  public class SettingsService : ISettingsService
  {
    public int GetInteger(string key, int defaultValue)
    {
      return key == "MinQuantityForBulkSave"
        ? 10
        : -1;
    }
  }
}