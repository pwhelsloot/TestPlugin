using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  /// <summary>
  /// This attribute allows a bsl trigger to be configured for an entity
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class BslTriggerEntityAttribute : Attribute
  {
    /// <summary>
    /// Allow this entity to be selected on platform ui when configuring a bsl trigger
    /// </summary>
    public bool AllowUISelection { get; set; }
  }
}
