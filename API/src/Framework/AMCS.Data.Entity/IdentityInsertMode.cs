using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public enum IdentityInsertMode
  {
    /// <summary>
    /// Let the database generate the Id.
    /// </summary>
    Off = 0,
    
    /// <summary>
    /// Explicitly define the Id of the entity.
    /// </summary>
    On = 1,

    /// <summary>
    /// Explicitly define the Id of the entity.
    /// This is required when the table does not have an identity column.
    /// </summary>
    OnWithOverride = 2,
  }
}