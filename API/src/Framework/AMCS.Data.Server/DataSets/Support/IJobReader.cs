using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Support
{
  public interface IJobReader
  {
    Job GetJobStatus(Guid id);
  }
}
