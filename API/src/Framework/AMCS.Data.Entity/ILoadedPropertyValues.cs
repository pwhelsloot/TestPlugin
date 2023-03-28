using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public interface ILoadedPropertyValues
  {
    void SetLoadedPropertyValues(object[] loadedPropertyValues);
  }
}
