using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AMCS.WebDiagnostics;

namespace AMCS.Data.Server.Services
{
  public class DefaultDiagnosticsRenderer : IDiagnosticsRenderer
  {
    private readonly DiagnosticsConfiguration diagnosticsConfiguration;

    public DefaultDiagnosticsRenderer()
      : this(CreateDiagnosticsConfiguration())
    {
    }

    public DefaultDiagnosticsRenderer(DiagnosticsConfiguration diagnosticsConfiguration)
    {
      this.diagnosticsConfiguration = diagnosticsConfiguration;
    }

    private static DiagnosticsConfiguration CreateDiagnosticsConfiguration()
    {
      var userService = DataServices.Resolve<IUserService>();

      return new DiagnosticsConfiguration(
        userService.ApplicationCode,
        new DiagnosticsVersion(userService.GetType().Assembly.GetName().Version.ToString(), null)
      );
    }

    public RenderedDiagnostics Render(DiagnosticsFormat format, bool run)
    {
      List<DiagnosticResult> diagnostics;

      if (run)
        diagnostics = DataServices.Resolve<IDiagnosticsManager>().GetResults();
      else
        diagnostics = new List<DiagnosticResult>();

      bool success = diagnostics.All(p => p is DiagnosticResult.Success);

      var output = DiagnosticsRenderer.Render(diagnosticsConfiguration, diagnostics, format);

      return new RenderedDiagnostics(
        output.Content,
        output.ContentType,
        success,
        Encoding.UTF8
      );
    }
  }
}
