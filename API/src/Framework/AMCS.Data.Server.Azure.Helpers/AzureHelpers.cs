namespace AMCS.Data.Server.Azure.Helpers
{
  using System;
  using AMCS.Data.Support.Security;

  public static class AzureHelpers
  {
    public const string WEBSITE_HOSTNAME = "WEBSITE_HOSTNAME";
    public const string WEBSITE_SITE_NAME = "WEBSITE_SITE_NAME";

    public static bool IsRunningOnAppService()
    {
      return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(WEBSITE_HOSTNAME));
    }

    public static string GetSiteName()
    {
      string enviromentName = Environment.GetEnvironmentVariable(WEBSITE_HOSTNAME);
      string siteName = null;
      if (!string.IsNullOrEmpty(enviromentName))
      {
        int charLocation = enviromentName.IndexOf("-svc", StringComparison.Ordinal);
        siteName = enviromentName.Substring(0, charLocation);
      }
      return siteName;
    }

    public static string GetSlotName()
    {
      var hostName = (Environment.GetEnvironmentVariable(WEBSITE_HOSTNAME).Split('.')[0]);
      var siteName = (Environment.GetEnvironmentVariable(WEBSITE_SITE_NAME));

      var slotName = hostName.Replace($"{siteName}-", string.Empty);
      return (!slotName.Equals(hostName)) ? slotName : null;
    }

    public static string GetExpectedDatabaseName()
    {
      string databaseName = null;
      if (IsRunningOnAppService())
      {
        string slot = GetSlotName();
        slot = (slot != null) ? $"-{slot}" : string.Empty;
        databaseName = $"{GetSiteName()}-sqldb-elemos{slot}";
      }

      return databaseName;
    }

    public static string GetFullSiteName()
    {
      string fullSiteName = null;

      string siteName = GetSiteName();
      if (!string.IsNullOrEmpty(siteName))
      {
        string slot = GetSlotName();
        slot = (slot != null) ? $"-{slot}" : string.Empty;
        fullSiteName = $"{siteName}{slot}";
      }

      return fullSiteName;
    }

    public static string GenerateInstanceName()
    {
      int maxServiceBusNameLength = 50;
      int randomStringLength = 30;
      var siteName = (Environment.GetEnvironmentVariable(WEBSITE_SITE_NAME));
      var randomString = RandomText.CreateKey(randomStringLength);
      if (string.IsNullOrEmpty(siteName))
      {
        return randomString;
      }
      else
      {
        var maxSiteNameLength = (maxServiceBusNameLength - randomStringLength - 1);
        if (siteName.Length > maxSiteNameLength)
          siteName = siteName.Remove(maxSiteNameLength);
        return $"{siteName}-{randomString}";
      }
    }
  }
}
