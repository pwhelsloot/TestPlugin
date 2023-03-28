using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration
{
  public interface ISetupService
  {
    void RegisterSetup(Action callback, int order = 0);
  }
}
