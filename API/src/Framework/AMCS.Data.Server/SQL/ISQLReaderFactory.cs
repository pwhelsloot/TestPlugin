using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal interface ISQLReaderFactory
  {
    string DefaultSearchResultId { get; }

    ISQLReaderResult GetReader();
  }
}
