using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Configuration;

namespace AMCS.Data.Server.Services
{
  public interface IDataSessionFactory
  {
    IDataSession GetDataSession(bool useReportingDatabase = false, bool? isDetached = false);

    IDataSession GetDataSession(ISessionToken userId, bool useReportingDatabase, bool? isDetached = false);

    IDataSession GetDataSession(IConnectionString connectionString, bool? isDetached = false);
  }
}
