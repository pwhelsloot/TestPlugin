using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  /// <summary>
  /// Classes that implement this interface can have different validation rules applied depending on context.
  /// </summary>
  public interface IValidatesInContext
  {
    ValidationContextEntity ValidationContext { get; }
  }
}
