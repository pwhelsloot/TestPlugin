using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  internal class EntityObjectReferenceMetadataChild : IEntityObjectReferenceMetadata
  {
    private readonly EntityChildAttribute attribute;

    public EntityChildAttribute Attribute => attribute;

    public EntityObjectReferenceMetadataChild(EntityChildAttribute attribute)
    {
      this.attribute = attribute;
    }

    public IEntityObjectReference CreateReference(EntityObjectProperty property, EntityObjectAccessor accessor)
    {
      var targetAccessor = EntityObjectAccessor.ForType(property.Type.GenericTypeArguments[0]);
      return new EntityObjectReferenceChild(accessor, property, targetAccessor);
    }

    public bool IsParentForChild(EntityChildAttribute childAttribute) => false;
  }
}
