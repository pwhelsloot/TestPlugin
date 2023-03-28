using System;
using System.IO;
using System.Net;
using AMCS.Data.Configuration;
using AMCS.Data.Support;
using log4net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AMCS.Data.Server.Services
{
  public class AzureBlobStorageService : IBlobStorageService
  {
    // If the blob is over 512 Kb in size, we write it to disk instead of
    // into a memory stream.
    private const int MaxMemoryStreamSize = 512 * 1024;

    private static readonly ILog Log = LogManager.GetLogger(typeof(AzureBlobStorageService));

    private readonly CloudBlobContainer container;

    public bool ExternalStorage { get; }

    public AzureBlobStorageService(IConnectionString connectionString, string container, bool enableExternalStorage)
    {
      if (!CloudStorageAccount.TryParse(connectionString.GetConnectionString(), out var storageAccount))
        throw new ArgumentException("Invalid connection string", nameof(connectionString));

      var client = storageAccount.CreateCloudBlobClient();

      this.container = client.GetContainerReference(container);

      if (!this.container.ExistsAsync().Result)
        this.container.CreateIfNotExistsAsync();

      ExternalStorage = enableExternalStorage;
    }

    public Stream GetBlob(string hash)
    {
      return TaskUtils.RunSynchronously(async () =>
      {
        try
        {
          return await container.GetBlockBlobReference(hash).OpenReadAsync();
        }
        catch (Exception ex)
        {
          Log.Error(ex);
          throw;
        }
      });
    }

    public string SaveBlob(Stream stream)
    {
      // To be able to create a blob in Azure, we need to know the name. This
      // means we can't use the trick we're using for the file system storage where
      // we calculate the hash while saving the file and then renaming it to the
      // correct name.
      //
      // We have a few paths here. If we can seek, we assume we can just rewind
      // after calculating the hash. Otherwise, depending on the size, we flush
      // the file to disk or to memory.

      if (stream.CanSeek)
        return SaveSeekableBlob(stream);

      // If the blob is small, we stream it into a memory stream. However, getting
      // the length may fail. In that case, we default to disk storage.

      try
      {
        if (stream.Length <= MaxMemoryStreamSize)
          return SaveSmallBlob(stream);
      }
      catch
      {
        // Ignore exceptions.
      }

      return SaveLargeBlob(stream);
    }

    private string SaveSeekableBlob(Stream stream)
    {
      using (stream)
      {
        // It could be that we're provided a stream that's not at its beginning.
        // We need to restore the correct position because otherwise the hash would
        // not describe the content we're actually uploading.

        long position = stream.Position;

        string hash = BlobStorageUtils.ComputeHash(stream);

        stream.Position = position;

        SaveBlob(stream, hash);

        return hash;
      }
    }

    private string SaveSmallBlob(Stream stream)
    {
      using (var target = new MemoryStream())
      {
        return SaveNonSeekableBlob(stream, target);
      }
    }

    private string SaveLargeBlob(Stream stream)
    {
      using (var tempFile = new TempFile())
      using (var target = tempFile.Create())
      {
        return SaveNonSeekableBlob(stream, target);
      }
    }

    private string SaveNonSeekableBlob(Stream stream, Stream target)
    {
      string hash;

      using (stream)
      {
        hash = BlobStorageUtils.CopyStreamComputeHash(stream, target);
      }

      target.Position = 0;
      target.Seek(0, SeekOrigin.Begin);

      SaveBlob(target, hash);

      return hash;
    }

    private void SaveBlob(Stream stream, string hash)
    {
      // The below access condition ensures an exception is thrown if the hash already
      // exists. We don't check for this because we assume most of the time it'll be a new
      // blob, it's not full proof anyway since the blob may have been created between
      // the check and the save, and it saves us a roundtrip.

      TaskUtils.RunSynchronously(async () =>
      {
        try
        {
          await container
            .GetBlockBlobReference(hash)
            .UploadFromStreamAsync(
              stream,
              AccessCondition.GenerateIfNoneMatchCondition("*"),
              null,
              null);
        }
        catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
        {
          // Ignore.
          Log.Warn(ex);
        }
        catch (Exception ex)
        {
          Log.Error(ex);
          throw;
        }
      });
    }
  }
}
