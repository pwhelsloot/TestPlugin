using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLQuery : ISQLParameterizedExecutable<ISQLQuery>
  {
    ISQLReadable Execute();

    ISQLExecutableResult ExecuteNonQuery();
  }
}
