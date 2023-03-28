using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetImportManager
  {
    int Concurrency { get; }

    string WriteFile(Stream stream);

    Stream ReadFile(string key);

    void DeleteFile(string key);

    IDataSetImportJob GetJob(Guid id);

    void DismissJob(Guid id);

    void CancelJob(Guid id);
  }
}
