using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public class ApiPropertyConfiguration
  {
    public static ApiPropertyConfiguration FromAttribute(ApiPropertyAttribute attribute)
    {
      return new ApiPropertyConfiguration(attribute.CollapseEmptyObject);
    }

    public bool CollapseEmptyObject { get; }

    public ApiPropertyConfiguration(bool collapseEmptyObject)
    {
      CollapseEmptyObject = collapseEmptyObject;
    }
  }
}
