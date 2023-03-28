namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Support
{
  using System;
  using System.Net;
  using AzureServiceBusSupport.RetryUtils;

  public static class DiagnosticsUtils
  {
    private static readonly BackoffProfile DefaultBackoffProfile = new BackoffProfile(TimeSpan.FromSeconds(1), 3, TimeSpan.FromSeconds(30));

    public static void RunDiagnostics(Uri url)
    { 
      Retryer.Retry(() => 
      {
        var request = WebRequest.CreateHttp(url);
        request.Timeout = 30000;

        using var response = (HttpWebResponse)request.GetResponse();
        if (response.StatusCode != HttpStatusCode.OK)
        {
          throw new Exception("Diagnostics failed");
        }
      }, 
      DefaultBackoffProfile);
    }
  }
}