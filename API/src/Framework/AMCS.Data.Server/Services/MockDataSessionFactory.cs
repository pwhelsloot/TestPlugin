using System;
using AMCS.Data.Configuration;

namespace AMCS.Data.Server.Services
{
  public class MockDataSessionFactory : IDataSessionFactory
  {
    private static StrictModeType? strictMode;
    private static readonly object syncRoot = new object();
    private readonly Func<IDataSession> dataSessionFactory;

    public static IDataSessionFactory For(IDataSession dataSession, StrictModeType strictModeType = StrictModeType.None)
    {
      return new MockDataSessionFactory(() => dataSession, strictModeType);
    }

    public static IDataSessionFactory For(Func<IDataSession> dataSessionFactory, StrictModeType strictModeType = StrictModeType.None)
    {
      return new MockDataSessionFactory(dataSessionFactory, strictModeType);
    }

    private MockDataSessionFactory(Func<IDataSession> dataSessionFactory, StrictModeType strictModeType)
    {
      this.dataSessionFactory = dataSessionFactory;

      lock (syncRoot)
      {
        if (strictMode == null)
        {
          strictMode = strictModeType;
          StrictMode.SetStrictMode(strictModeType);
        }
      }
    }

    public IDataSession GetDataSession(bool useReportingDatabase = false, bool? isDetached = false)
    {
      return dataSessionFactory();
    }

    public IDataSession GetDataSession(ISessionToken userId, bool useReportingDatabase, bool? isDetached = false)
    {
      return dataSessionFactory();
    }

    public IDataSession GetDataSession(IConnectionString connectionString, bool? isDetached = false)
    {
      return dataSessionFactory();
    }
  }
}
