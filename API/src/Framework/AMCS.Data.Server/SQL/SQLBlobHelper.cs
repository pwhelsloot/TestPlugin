using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.Services;

namespace AMCS.Data.Server.SQL
{
  internal static class SQLBlobHelper
  {
    public static Stream GetBlob(EntityBlob blob, IDataSession dataSession)
    {
      GetBlobStreamOrBuffer(blob, dataSession, out var stream, out var buffer);

      if (buffer != null)
        return new MemoryStream(buffer);

      return stream;
    }

    public static byte[] GetBlobAsArray(EntityBlob blob, IDataSession dataSession)
    {
      GetBlobStreamOrBuffer(blob, dataSession, out var stream, out var buffer);

      if (buffer != null)
        return buffer;

      if (stream != null)
      {
        using (stream)
        using (var target = new MemoryStream())
        {
          stream.CopyTo(target);

          return target.ToArray();
        }
      }

      return null;
    }

    private static void GetBlobStreamOrBuffer(EntityBlob blob, IDataSession dataSession, out Stream stream, out byte[] buffer)
    {
      stream = null;
      buffer = null;

      var accessor = EntityObjectAccessor.ForType(blob.Metadata.EntityType);

      // If there is a hash, there MUST be a storage system configured. We're not
      // checking whether that service has external storage configured.

      string hash = (string)accessor.GetProperty(blob.Metadata.HashMemberName).GetValue(blob.EntityObject);
      if (!string.IsNullOrEmpty(hash))
      {
        stream = DataServices.Resolve<IBlobStorageService>().GetBlob(hash);
        return;
      }

      // Otherwise, try to get it from the database. We always do this to ensure that
      // if the database is just partially migrated, we can always get the blob.
      // The disadvantage is we're running an extra query even if the record never
      // has the blob. This however is a very simple, fast, query of a single field by
      // primary key, so the impact should be very low of this.

      buffer = GetBlobFromDatabase(accessor, blob, dataSession);
    }

    private static byte[] GetBlobFromDatabase(EntityObjectAccessor accessor, EntityBlob blob, IDataSession dataSession)
    {
      // For inherited entities, we need to go through the key name of the base class,
      // e.g. WeighingPictureEntity inherits from PictureEntity. KeyName here will
      // return PictureId, which is not what Id32 would return.
      int? id = (int?)accessor.GetPropertyByColumnName(accessor.KeyName).GetValue(blob.EntityObject);
      if (id.GetValueOrDefault() <= 0)
        return null;

      var tb = new SQLTextBuilder()
        .Text("SELECT ").Name(blob.Metadata.BlobColumnName).Text(" ")
        .Text("FROM ").TableName(accessor).Text(" ")
        .Text("WHERE ").Name(accessor.KeyName).Text(" = ").ParameterName("@Id");

      return dataSession.Query(tb.ToString())
        .Set("@Id", id)
        .Execute()
        .FirstOrDefaultScalar<byte[]>();
    }

    public static void SaveBlob(EntityBlob blob, byte[] buffer)
    {
      if (buffer != null)
      {
        using (var stream = new MemoryStream(buffer))
        {
          SaveBlob(blob, stream);
        }
      }
    }

    public static void SaveBlob(EntityBlob blob, Stream stream)
    {
      var service = DataServices.Resolve<IBlobStorageService>();

      // If the service doesn't store blobs externally, queue the blob to store
      // it into the database.

      if (!service.ExternalStorage)
      {
        SaveBlobToDatabase(blob, stream);
        return;
      }

      // Otherwise, store it into the external system. The ORM layer will take care
      // of NULL-ing the blob field if there's a hash.

      string hash = service.SaveBlob(stream);

      var accessor = EntityObjectAccessor.ForType(blob.Metadata.EntityType);

      accessor.GetProperty(blob.Metadata.HashMemberName).SetValue(blob.EntityObject, hash);
    }

    private static void SaveBlobToDatabase(EntityBlob blob, Stream stream)
    {
      byte[] buffer;

      if (stream is MemoryStream memoryStream)
      {
        buffer = memoryStream.ToArray();
      }
      else
      {
        using (var target = new MemoryStream())
        {
          stream.CopyTo(target);

          buffer = target.ToArray();
        }
      }

      // We're not clearing the hash here because we don't have to. The only
      // way there could be a hash set would be if the system was configured
      // with external storage before, and now isn't. That would break
      // everything already.

      blob.SetPendingBlob(buffer);
    }

    public static void ClearBlob(EntityBlob blob)
    {
      // Clear the hash member and mark the pending content as clear blob.
      // The ORM layer will understand that this means that the blob field
      // will have to be set to NULL.

      var accessor = EntityObjectAccessor.ForType(blob.Metadata.EntityType);

      accessor.GetProperty(blob.Metadata.HashMemberName).SetValue(blob.EntityObject, null);

      blob.SetPendingBlob(null);
    }
  }
}
