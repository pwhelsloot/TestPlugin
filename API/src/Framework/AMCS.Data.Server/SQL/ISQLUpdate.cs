using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLUpdate : ISQLExecutable<ISQLUpdate>
  {
    ISQLUpdate CreateAuditRecord();

    ISQLUpdate CreateAuditRecord(bool value);

    ISQLUpdate IgnoreSpecialFields();

    ISQLUpdate IgnoreSpecialFields(bool value);

    ISQLUpdate UpdateOverridableDynamicColumns();

    ISQLUpdate UpdateOverridableDynamicColumns(bool value);

    ISQLUpdate SpecialFields(IList<string> fields);

    ISQLUpdate RestrictToFields(IList<string> fields);

    void Execute();
  }
}
