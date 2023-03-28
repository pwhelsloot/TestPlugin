using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  internal interface IEntityObjectReferenceMetadata
  {
    IEntityObjectReference CreateReference(EntityObjectProperty property, EntityObjectAccessor accessor);
    bool IsParentForChild(EntityChildAttribute childAttribute);
  }
}
