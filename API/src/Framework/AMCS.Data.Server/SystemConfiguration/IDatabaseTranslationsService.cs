namespace AMCS.Data.Server.SystemConfiguration
{
  using System.Collections.Generic;

  public interface IDatabaseTranslationsService
  {
    Dictionary<string, string> GetTranslations(string languageCode);
  }
}
