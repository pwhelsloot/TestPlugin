using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.WebDiagnostics;

namespace AMCS.Data.Server.Services
{
  public interface IDiagnosticsRenderer
  {
    RenderedDiagnostics Render(DiagnosticsFormat format, bool run);
  }
}
