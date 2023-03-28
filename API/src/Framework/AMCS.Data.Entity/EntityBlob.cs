using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public class EntityBlob
  {
    public EntityObject EntityObject { get; }

    public EntityBlobMetadata Metadata { get; }

    public EntityBlob(EntityObject entityObject, EntityBlobMetadata metadata)
    {
      Debug.Assert(metadata.EntityType.IsInstanceOfType(entityObject));

      EntityObject = entityObject;
      Metadata = metadata;
    }

    public void SetPendingBlob(byte[] data)
    {
      EntityObject.SetPendingBlob(this, data);
    }

    public bool TryGetPendingBlob(out byte[] data, bool remove = false)
    {
      return EntityObject.TryGetPendingBlob(this, out data, remove);
    }
  }
}
