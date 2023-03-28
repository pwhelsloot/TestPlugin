using System;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using AMCS.Data.Entity.SQL;
using AMCS.Data.Server.SQL;

namespace AMCS.Data.Server.Services
{
  public class SQLDataSessionFactory : IDataSessionFactory
  {
    public const string InvalidSessionExceptionIdentifier = "INVALID_SESSION";

    private IDataEvents events;
    private IDataMetricsEvents metrics;

    public SQLDataSessionConfiguration Configuration { get; }

    public SQLDataSessionFactory(SQLDataSessionConfiguration configuration, ISetupService setupService,
        IDataEventsBuilderService dataEventsBuilderService,
        IDataMetricsEventsBuilderService dataMetricsEventsBuilderService)
    {
      this.Configuration = configuration;
      
      setupService.RegisterSetup(() => events = dataEventsBuilderService.Build() ?? EmptyDataEvents.Instance, -2000);
      setupService.RegisterSetup(() => metrics = dataMetricsEventsBuilderService.Build(), -2000);

      SQLAzureCompatibility.SetAzureCompatibilityEnabled(Configuration.IsAzureCompatibilityEnabled);
      SQLPerformanceLogger.SetEnabled(Configuration.IsPerformanceMonitoringEnabled);
    }

    public IDataSession GetDataSession(IConnectionString connectionString, bool? isDetached = null)
    {
      return new SQLDataSession(connectionString, Configuration, events, metrics, isDetached);
    }

    public IDataSession GetDataSession(bool useReportingDatabase, bool? isDetached = null)
    {
      return GetDataSession(
        useReportingDatabase ? Configuration.ReportingConnectionString : Configuration.ConnectionString, isDetached);
    }

    public IDataSession GetDataSession(ISessionToken userId, bool useReportingDatabase, bool? isDetached = null)
    {
      if (userId == null)
        throw new Exception("No session key was provided and therefore a data session could not be created.");

      return GetDataSession(useReportingDatabase, isDetached);
    }

    private class EmptyDataEvents : IDataEvents
    {
      public static readonly EmptyDataEvents Instance = new EmptyDataEvents();

      private EmptyDataEvents()
      {
      }

      public void AfterInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
      }

      public void AfterUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
      }

      public void AfterDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
      }

      public void BeforeInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity)
      {
      }

      public void BeforeUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
      }

      public void BeforeDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
      }
    }
  }
}
