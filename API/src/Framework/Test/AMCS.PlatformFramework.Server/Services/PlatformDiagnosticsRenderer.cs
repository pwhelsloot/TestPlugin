using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Services;
using AMCS.WebDiagnostics;

namespace AMCS.PlatformFramework.Server.Services
{
  public class PlatformDiagnosticsRenderer : DefaultDiagnosticsRenderer
  {
    public PlatformDiagnosticsRenderer()
      : base(CreateDiagnosticsConfiguration())
    {
    }

    private static DiagnosticsConfiguration CreateDiagnosticsConfiguration()
    {
      return new DiagnosticsConfiguration("Platform Template", GetVersions().ToArray());
    }

    private static IEnumerable<DiagnosticsVersion> GetVersions()
    {
      yield return new DiagnosticsVersion(typeof(PlatformDiagnosticsRenderer).Assembly.GetName().Version.ToString(), "Server");
    }
  }
}
