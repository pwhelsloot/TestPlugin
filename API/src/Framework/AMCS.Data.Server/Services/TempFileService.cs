using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Support.Security;
using log4net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AMCS.Data.Server.Services
{
  internal class TempFileService : ITempFileService, IDisposable
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(TempFileService));

    private readonly IStorage storage;
    private Timer timer;
    private bool disposed;

    public TempFileService(string blobStorageContainer, string filePath, TimeSpan timeToLive, IConnectionString blobStorageConnectionString)
    {
      if (!string.IsNullOrEmpty(blobStorageContainer))
      {
        storage = new BlobStorage(blobStorageContainer, blobStorageConnectionString, timeToLive);
      }
      else
      {
        if (string.IsNullOrEmpty(filePath))
        {
          var random = SharedRandom.GetRandom();

          while (true)
          {
            filePath = Path.Combine(Path.GetTempPath(), $"PlatformTmp-{random.Next(100000, 999999)}");
            if (!Directory.Exists(filePath))
            {
              Directory.CreateDirectory(filePath);
              break;
            }
          }
        }

        storage = new FileStorage(filePath, timeToLive);
      }

      timer = new Timer(TimerCallback, null, new TimeSpan(), TimeSpan.FromHours(1));
    }
    public TempFileService(ITempFileStorageConfiguration configuration, IConnectionString blobStorageConnectionString)
      :this(configuration?.Azure?.Container, configuration?.FileSystem?.Store, configuration?.Ttl ?? TimeSpan.FromDays(7), blobStorageConnectionString)
    {      
    }

    private void TimerCallback(object state)
    {
      try
      {
        storage.DeleteExpiredFiles();
      }
      catch (Exception ex)
      {
        Log.Info("Could not delete expired files from storage", ex);
      }
    }

    public string WriteFile(Stream stream)
    {
      return storage.WriteFile(stream);
    }

    public Task<string> WriteFileAsync(Stream stream)
    {
      return storage.WriteFileAsync(stream);
    }

    public Stream ReadFile(string key)
    {
      return storage.ReadFile(key);
    }

    public Task<Stream> ReadFileAsync(string key)
    {
      return storage.ReadFileAsync(key);
    }

    public void DeleteFile(string key)
    {
      storage.DeleteFile(key);
    }

    public Task DeleteFileAsync(string key)
    {
      return storage.DeleteFileAsync(key);
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (timer != null)
        {
          timer.Dispose();
          timer = null;
        }

        disposed = true;
      }
    }

    private interface IStorage
    {
      string WriteFile(Stream stream);

      Task<string> WriteFileAsync(Stream stream);

      Stream ReadFile(string key);

      Task<Stream> ReadFileAsync(string key);

      void DeleteFile(string key);

      Task DeleteFileAsync(string key);

      void DeleteExpiredFiles();
    }

    private class FileStorage : IStorage
    {
      private readonly string path;
      private readonly TimeSpan ttl;

      public FileStorage(string path, TimeSpan ttl)
      {
        this.path = path;
        this.ttl = ttl;

        Directory.CreateDirectory(path);
      }

      public string WriteFile(Stream stream)
      {
        string key = RandomText.CreateKey(30);

        using (var target = File.Create(Path.Combine(path, key)))
        {
          stream.CopyTo(target);
        }

        return key;
      }

      public async Task<string> WriteFileAsync(Stream stream)
      {
        string key = RandomText.CreateKey(30);

        using (var target = File.Create(Path.Combine(path, key)))
        {
          await stream.CopyToAsync(target);
        }

        return key;
      }

      public Stream ReadFile(string key)
      {
        return File.OpenRead(Path.Combine(path, key));
      }

      public Task<Stream> ReadFileAsync(string key)
      {
        return Task.FromResult(ReadFile(key));
      }

      public void DeleteFile(string key)
      {
        File.Delete(Path.Combine(path, key));
      }

      public Task DeleteFileAsync(string key)
      {
        DeleteFile(key);

        return Task.CompletedTask;
      }

      public void DeleteExpiredFiles()
      {
        var expiration = DateTimeOffset.Now - ttl;

        foreach (string file in Directory.GetFiles(path))
        {
          if (File.GetCreationTime(file) < expiration)
            File.Delete(file);
        }
      }
    }

    private class BlobStorage : IStorage
    {
      private readonly CloudBlobContainer container;
      private readonly TimeSpan ttl;

      public BlobStorage(string container, IConnectionString connectionString, TimeSpan ttl)
      {
        if (!CloudStorageAccount.TryParse(connectionString.GetConnectionString(), out var storageAccount))
          throw new ArgumentException("Invalid connection string", nameof(connectionString));

        var client = storageAccount.CreateCloudBlobClient();

        this.container = client.GetContainerReference(container);
        this.ttl = ttl;

        if (!this.container.ExistsAsync().Result)
          this.container.CreateIfNotExistsAsync().Wait();
      }

      public string WriteFile(Stream stream)
      {
        string key = RandomText.CreateKey(30);

        container.GetBlockBlobReference(key).UploadFromStreamAsync(stream).Wait();

        return key;
      }

      public async Task<string> WriteFileAsync(Stream stream)
      {
        string key = RandomText.CreateKey(30);

        await container.GetBlockBlobReference(key).UploadFromStreamAsync(stream);

        return key;
      }

      public Stream ReadFile(string key)
      {
        return container.GetBlockBlobReference(key).OpenReadAsync().Result;
      }

      public Task<Stream> ReadFileAsync(string key)
      {
        return container.GetBlockBlobReference(key).OpenReadAsync();
      }

      public void DeleteFile(string key)
      {
        container.GetBlockBlobReference(key).DeleteIfExistsAsync().Wait();
      }

      public Task DeleteFileAsync(string key)
      {
        return container.GetBlockBlobReference(key).DeleteIfExistsAsync();
      }

      public void DeleteExpiredFiles()
      {
        DeleteExpiredFilesAsync().Wait();
      }

      private async Task DeleteExpiredFilesAsync()
      {
        var expiration = DateTimeOffset.Now - ttl;

        foreach (var blob in container.ListBlobsSegmentedAsync(null).Result.Results.OfType<CloudBlob>())
        {
          if (blob.Properties.LastModified < expiration)
          {
            await blob.DeleteAsync();
          }
        }
      }
    }
  }
}
