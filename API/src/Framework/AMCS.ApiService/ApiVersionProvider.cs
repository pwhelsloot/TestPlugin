using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AMCS.ApiService
{
  public class ApiVersionProvider
  {
    public Version CurrentVersion { get; }
    public Version LatestVersion { get; }

    private ApiVersionProvider(Version currentVersion, Version latestVersion)
    {
      this.CurrentVersion = currentVersion;
      this.LatestVersion = latestVersion;
    }

    public static ApiVersionProvider LoadVersions(Assembly assembly, string resourceName)
    {
      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        var apiMetaData= XDocument.Load(stream);
        var currentVersion = apiMetaData.Root.Attribute("CurrentVersion").Value;
        var latestVersion = apiMetaData.Root.Attribute("LatestVersion").Value;
        
        return new ApiVersionProvider(new Version(currentVersion), new Version(latestVersion));
      }
    }
  }
}
