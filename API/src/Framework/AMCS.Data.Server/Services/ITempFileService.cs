using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public interface ITempFileService
  {
    string WriteFile(Stream stream);

    Task<string> WriteFileAsync(Stream stream);

    Stream ReadFile(string key);

    Task<Stream> ReadFileAsync(string key);

    void DeleteFile(string key);

    Task DeleteFileAsync(string key);
  }
}
