using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ApiPropertyAttribute : Attribute
  {
    /// <summary>
    /// Collapse empty objects as a null reference when writing JSON,
    /// and create an empty instance when reading a null reference.
    /// </summary>
    public bool CollapseEmptyObject { get; set; }
  }
}
