namespace AMCS.TestPlugin.Server.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Server.Services;
  using AMCS.WebDiagnostics;
  
  public class PlatformDiagnosticsRenderer : DefaultDiagnosticsRenderer
  {
    public PlatformDiagnosticsRenderer()
      : base(CreateDiagnosticsConfiguration())
    {
    }

    private static DiagnosticsConfiguration CreateDiagnosticsConfiguration()
    {
      return new DiagnosticsConfiguration("TestPlugin", GetVersions().ToArray());
    }

    private static IEnumerable<DiagnosticsVersion> GetVersions()
    {
      yield return new DiagnosticsVersion(typeof(PlatformDiagnosticsRenderer).Assembly.GetName().Version.ToString(), "Server");
    }
  }
}
