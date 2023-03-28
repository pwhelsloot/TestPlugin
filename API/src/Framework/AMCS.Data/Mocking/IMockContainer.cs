using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Mocking
{
  internal interface IMockContainer
  {
    object Resolve(Type type);

    bool TryResolve(Type type, out object instance);
  }
}
