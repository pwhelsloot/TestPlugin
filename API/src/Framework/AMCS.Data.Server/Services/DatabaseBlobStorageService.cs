using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class DatabaseBlobStorageService : IBlobStorageService
  {
    public bool ExternalStorage => false;

    public Stream GetBlob(string hash)
    {
      throw new NotSupportedException();
    }

    public string SaveBlob(Stream stream)
    {
      throw new NotSupportedException();
    }
  }
}
