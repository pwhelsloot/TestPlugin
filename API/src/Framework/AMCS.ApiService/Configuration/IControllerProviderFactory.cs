using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Configuration
{
  public interface IControllerProviderFactory
  {
    IControllerProvider Create();
  }
}
