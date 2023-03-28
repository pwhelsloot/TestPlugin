using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Parallel
{
  public interface IParallelTask
  {
    void ExecuteWith();

    void ExecuteRun(IDataSession dataSession);

    void ExecuteCollect();
  }
}
