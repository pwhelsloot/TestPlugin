using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public enum BslAction
  {
    Create,
    Update,
    Delete,
    BeforeCreate,
    BeforeUpdate,
    BeforeDelete
  }
}