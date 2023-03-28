using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public class ApiExplorerAttribute : Attribute
  {
    public ApiExplorerMethods Methods { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }

    public Type ContractProvider { get; set; }

    public ApiMode Mode { get; set; }

    public string UserRole { get; set; }
  }
}
