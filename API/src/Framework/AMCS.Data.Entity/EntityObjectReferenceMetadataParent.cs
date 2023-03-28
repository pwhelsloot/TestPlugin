using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  internal class EntityObjectReferenceMetadataParent : IEntityObjectReferenceMetadata
  {
    private readonly EntityParentAttribute attribute;

    public EntityParentAttribute Attribute => attribute;

    public EntityObjectReferenceMetadataParent(EntityParentAttribute attribute)
    {
      this.attribute = attribute;
    }

    public IEntityObjectReference CreateReference(EntityObjectProperty property, EntityObjectAccessor accessor)
    {
      return new EntityObjectReferenceParent(property);
    }

    public bool IsParentForChild(EntityChildAttribute childAttribute)
    {
      return attribute.ForeignKeyColumn == childAttribute.ForeignKeyColumn;
    }
  }
}
