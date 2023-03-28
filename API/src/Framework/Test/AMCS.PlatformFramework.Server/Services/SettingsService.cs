using AMCS.Data.Server.Services;

namespace AMCS.PlatformFramework.Server.Services
{
  public class SettingsService : ISettingsService
  {
    public int GetInteger(string key, int defaultValue)
    {
      return defaultValue;
    }
  }
}