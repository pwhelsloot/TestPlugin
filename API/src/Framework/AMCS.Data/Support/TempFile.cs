using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
  /// <summary>
  /// Represents a temporary file.
  /// </summary>
  /// <remarks>
  /// Implements a simple wrapper to create temporary files and have
  /// them deleted automatically when the instance is disposed.
  /// </remarks>
  public class TempFile : IDisposable
  {
    private bool disposed;

    public string FileName { get; }

    public TempFile()
    {
      FileName = Path.GetTempFileName();
    }

    public Stream Create()
    {
      return File.Create(FileName);
    }

    public Stream OpenRead()
    {
      return File.OpenRead(FileName);
    }

    public void Dispose()
    {
      if (!disposed)
      {
        try
        {
          File.Delete(FileName);
        }
        catch
        {
          // Ignore exceptions.
        }

        disposed = true;
      }
    }
  }
}
