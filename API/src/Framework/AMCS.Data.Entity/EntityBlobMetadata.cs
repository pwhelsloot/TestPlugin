using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public class EntityBlobMetadata
  {
    public static EntityBlobMetadata Create<T>(string blobMemberName, string blobColumnName, string hashMemberName)
      where T : EntityObject
    {
      return new EntityBlobMetadata(typeof(T), blobMemberName, blobColumnName, hashMemberName);
    }

    public Type EntityType { get; }

    public string BlobMemberName { get; }

    public string BlobColumnName { get; }

    public string HashMemberName { get; }

    private EntityBlobMetadata(Type entityType, string blobMemberName, string blobColumnName, string hashMemberName)
    {
      EntityType = entityType;
      BlobMemberName = blobMemberName;
      BlobColumnName = blobColumnName;
      HashMemberName = hashMemberName;
    }
  }
}
