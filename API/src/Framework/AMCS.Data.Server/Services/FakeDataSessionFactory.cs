using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Configuration;

namespace AMCS.Data.Server.Services
{
  public class FakeDataSessionFactory : IDataSessionFactory
  {
    public FakeDataSessionFactory(StrictModeType strictMode)
    {
      StrictMode.SetStrictMode(strictMode);
    }

    public IDataSession GetDataSession(IConnectionString connectionString, bool? isDetached = false)
    {
      return new FakeDataSession();
    }

    public IDataSession GetDataSession(bool useReportingDatabase, bool? isDetached = false)
    {
      return GetDataSession(null);
    }

    public IDataSession GetDataSession(ISessionToken userId, bool useReportingDatabase, bool? isDetached = false)
    {
      return GetDataSession(null);
    }
  }
}
