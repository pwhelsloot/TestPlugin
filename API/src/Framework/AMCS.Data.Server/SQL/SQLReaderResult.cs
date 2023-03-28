using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal interface ISQLReaderResult : IDisposable
  {
    IDataReader GetReader();

    ISQLExecutableResult GetResult();
  }
}
