using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Interfaces
{
  public interface IEntityRestrictionsEntity
  {
    bool IsNewDenied { get; set; }

    bool IsEditDenied { get; set; }

    bool IsDeleteDenied { get; set; }
  }
}
