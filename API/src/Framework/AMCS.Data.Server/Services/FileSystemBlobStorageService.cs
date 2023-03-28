using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support.Security;

namespace AMCS.Data.Server.Services
{
  public class FileSystemBlobStorageService : IBlobStorageService
  {
    private readonly Random random = SharedRandom.GetRandom();
    private readonly string store;
    private readonly object syncRoot = new object();

    public bool ExternalStorage { get; }

    public FileSystemBlobStorageService(string store, bool enableExternalStorage)
    {
      this.store = store;

      ExternalStorage = enableExternalStorage;
    }

    public Stream GetBlob(string hash)
    {
      // We assume that the file exists if a hash is provided, since we never
      // delete documents from blob storage.

      return File.OpenRead(BuildStorePath(hash));
    }

    public string SaveBlob(Stream stream)
    {
      // We assume the default case is that we're actually are going to have to
      // write the blob to disk. To optimize for this, we calculate the hash
      // while writing the file to a temp file. If we have the hash already,
      // we delete the file. Otherwise we move the file into the correct location.

      string tempFile;
      string hash;

      // Copy the stream into a temp file, calculating its hash.

      using (stream)
      using (var target = CreateTempFile(out tempFile))
      {
        hash = BlobStorageUtils.CopyStreamComputeHash(stream, target);

        target.Flush(true);
      }

      // Try to move the temp file into the target file name. If this fails, we check
      // whether the proper file appeared and rethrow the exception if it didn't. We just
      // assume that if the file is there, it's all fine.

      string targetFileName = BuildStorePath(hash);

      Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));

      try
      {
        File.Move(tempFile, targetFileName);
      }
      catch
      {
        if (!File.Exists(targetFileName))
          throw;
      }

      // File.Move doesn't throw if the file already exists, and File.Delete doesn't
      // delete if the file is not there. As such, this is safe to do.

      File.Delete(tempFile);

      return hash;
    }

    private string BuildStorePath(string hash)
    {
      // Git uses just one level of directories. However, they have a cleanup algorithm
      // that should keep the number of files in the store relatively small. We don't,
      // so we need to be prepared for a large number of files. Two levels should be fine.

      return Path.Combine(
        store,
        hash.Substring(0, 2),
        hash.Substring(2, 2),
        hash.Substring(4)
      );
    }

    private FileStream CreateTempFile(out string fileName)
    {
      // The algorithm below creates a new temporary file in the store directory.
      // It does this by generating a random number and opening the file as CreateNew.
      // The chances this every has to retry is slim already, but we do retry
      // a maximum of 10 times. We need to have a limit here because the directory
      // could be unwritable.

      int tries = 10;

      while (true)
      {
        int random;

        lock (syncRoot)
        {
          random = this.random.Next();
        }

        fileName = Path.Combine(store, $"~{random}.tmp");

        try
        {
          return File.Open(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        }
        catch (IOException)
        {
          if (--tries <= 0)
            throw;
        }
      }
    }
  }
}
