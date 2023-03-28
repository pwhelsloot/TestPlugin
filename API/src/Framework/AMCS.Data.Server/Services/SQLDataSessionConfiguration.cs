using AMCS.Data.Configuration;

namespace AMCS.Data.Server.Services
{
  public class SQLDataSessionConfiguration
  {
    public IConnectionString ConnectionString { get; }
    public IConnectionString ReportingConnectionString { get; }
    public int? CommandTimeout { get; }
    public int? CommandTimeoutExtended { get; }
    public int? BulkCopyTimeout { get; }
    public bool IsPerformanceMonitoringEnabled { get; }
    public bool IsAzureCompatibilityEnabled { get; }
    public int? ParallelDataSessionThreadCount { get; }

    public bool IsAuditTableEnable { get; }

    public SQLDataSessionConfiguration(IConnectionString connectionString, IConnectionString reportingConnectionString,
      int? commandTimeout, int? commandTimeoutExtended, int? bulkCopyTimeout,
      bool isPerformanceMonitoringEnabled, bool isAzureCompatibilityEnabled, int? parallelDataSessionThreadCount,
      bool isAuditTableEnabled)
    {
      ConnectionString = connectionString;
      ReportingConnectionString = reportingConnectionString;
      CommandTimeout = commandTimeout;
      CommandTimeoutExtended = commandTimeoutExtended;
      BulkCopyTimeout = bulkCopyTimeout;
      IsPerformanceMonitoringEnabled = isPerformanceMonitoringEnabled;
      IsAzureCompatibilityEnabled = isAzureCompatibilityEnabled;
      ParallelDataSessionThreadCount = parallelDataSessionThreadCount;
      IsAuditTableEnable = isAuditTableEnabled;
    }
  }
}
