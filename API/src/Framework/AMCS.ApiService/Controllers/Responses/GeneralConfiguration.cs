using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class GeneralConfiguration : IPlatformUIGeneralConfiguration
  {
    private readonly IPlatformUIGeneralConfiguration wrapped;

    public GeneralConfiguration(IPlatformUIGeneralConfiguration wrapped)
    {
      this.wrapped = wrapped;
    }

    public bool IsTabControlReadOnly => wrapped.IsTabControlReadOnly;

    public bool HideSiteOrderActions => wrapped.HideSiteOrderActions;
  }
}
