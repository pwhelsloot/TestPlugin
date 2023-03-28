using System;

namespace AMCS.Data.Server.Api
{
  public static class NetworkUtils
  {
    public static string GetBaseDomain(string domainName)
    {
      var splitTokens = domainName.Split('.');

      // only split 3 segments like www.west-wind.com
      return splitTokens.Length != 3 
        ? domainName 
        : $"{splitTokens[splitTokens.Length - 2]}.{splitTokens[splitTokens.Length - 1]}";
    }

    public static string GetBaseDomain(this Uri uri)
    {
      return uri.HostNameType == UriHostNameType.Dns 
        ? GetBaseDomain(uri.DnsSafeHost) 
        : uri.Host;
    }
  }
}