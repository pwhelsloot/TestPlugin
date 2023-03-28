using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLDelete : ISQLExecutable<ISQLDelete>
  {
    ISQLDelete CreateAuditRecord();

    ISQLDelete CreateAuditRecord(bool value);

    ISQLDelete Undelete();

    ISQLDelete Undelete(bool value);

    bool Execute();
  }
}
