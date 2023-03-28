using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public interface IBlobStorageService
  {
    bool ExternalStorage { get; }

    Stream GetBlob(string hash);

    string SaveBlob(Stream stream);
  }
}
