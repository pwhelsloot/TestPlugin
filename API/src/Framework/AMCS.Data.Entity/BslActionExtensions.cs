using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public static class BslActionExtensions
  {
    public static bool IsCreateOrUpdate(this BslAction bslAction)
    {
      return bslAction == BslAction.Create || bslAction == BslAction.Update;
    }
  }
}
