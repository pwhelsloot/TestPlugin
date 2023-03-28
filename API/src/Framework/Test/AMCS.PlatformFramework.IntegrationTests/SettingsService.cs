using AMCS.Data.Server.Services;

namespace AMCS.PlatformFramework.IntegrationTests
{
  public class SettingsService : ISettingsService
  {
    public const int MinQuantityForBulkSave = 10;

    public int GetInteger(string key, int defaultValue)
    {
      if (key == nameof(MinQuantityForBulkSave))
        return MinQuantityForBulkSave;

      return defaultValue;
    }
  }
}