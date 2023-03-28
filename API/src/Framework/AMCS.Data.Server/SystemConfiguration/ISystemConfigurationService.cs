namespace AMCS.Data.Server.SystemConfiguration
{
  using System.Collections.Generic;

  public interface ISystemConfigurationService
  {
    string XsdSchema { get; }

    IExportResult LoadConfiguration(ISessionToken userId);

    ISaveResult SaveConfiguration(ISessionToken userId, string xml, List<string> xmlOverrides = null, string databaseLanguage = "");

    IValidationResult ValidateConfiguration(IConfiguration configuration);
  }
}
