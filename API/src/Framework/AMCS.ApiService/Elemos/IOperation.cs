using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos
{
  public interface IOperation
  {
    string Name { get; }

    Type Handler { get; }
  }
}
