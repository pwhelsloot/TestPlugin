using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AMCS.Data;
using AMCS.Data.EndToEnd.Tests;
using AMCS.Data.EndToEnd.Tests.DataMetrics;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.Data.EndToEnd.Tests
{
  [Binding]
  public class ScenarioSetup : BindingBase
  {
    public static ISessionToken SessionKey { get; private set; }

    public static string PrivateKey { get; private set; }

    public ScenarioSetup(ScenarioContext context)
      : base(context)
    {
      Login();
    }

    private void Login()
    {
      if (SessionKey == null)
      {
        var loginManager = DataServices.Resolve<ILoginManager>();
        SessionKey = loginManager.Login();
        PrivateKey = loginManager.GeneratePrivateKey(SessionKey);
      }
    }

    /// <summary>
    /// Before each Scenario set Tag
    /// </summary>
    [BeforeScenario(Order = int.MinValue)]
    public void BeforeScenario()
    {
      SetTag();
    }

    /// <summary>
    /// Before each ScenarioBlock start new datasession and transaction
    /// If scenario block is type When also set limits
    /// </summary>
    [BeforeScenarioBlock]
    public void BeforeScenarioBlock()
    {
      if (Context.CurrentScenarioBlock == ScenarioBlock.When)
      {
        var openConnectionsLimit = GetLimit("OpenConnections", 1);
        var limits = new TestDataMetricsLimits
        {
          OpenConnections = openConnectionsLimit,
          OpenDetachedConnections = GetLimit("OpenDetachedConnections", null),
          TransactionCommits = GetLimit("TransactionCommits", openConnectionsLimit),
          TransactionRollbacks = GetLimit("TransactionRollbacks", 0),
          TransactionStarts = GetLimit("TransactionStarts", 1),
          Queries = GetLimit("Queries", 50),
          ReaderRows = GetLimit("ReaderRows", 1000),
          Inserts = GetLimit("Inserts", 100),
          Updates = GetLimit("Updates", 100),
          Deletes = GetLimit("Deletes", 500)
        };

        DataServices.Resolve<ITestDataMetricsEventsService>().Reset(limits);
      }

      StartSession(SessionKey, PrivateKey);
      Session.StartTransaction();
    }

    /// <summary>
    /// After each ScenarioBlock end transaction and dispose session
    /// If scenario block is type When also write data metrics in log and only write events if debugging
    /// </summary>
    [AfterScenarioBlock]
    public void AfterScenarioBlock()
    {
      if (Session.IsTransaction())
      {
        Session.CommitTransaction();
      }
      DisposeSession();
      var metricsService = DataServices.Resolve<ITestDataMetricsEventsService>();

      if (Context.CurrentScenarioBlock != ScenarioBlock.When)
      {
        metricsService.Reset();
        return;
      }

      TestContext.WriteLine("Used tag: " + Tag);

      TestContext.WriteLine("========== Metrics ==========");

      TestContext.WriteLine("Connections opened : " + metricsService.OpenConnections);
      TestContext.WriteLine("Detached connections opened : " + metricsService.OpenDetachedConnections);
      TestContext.WriteLine("Connections closed : " + metricsService.ClosedConnections);
      TestContext.WriteLine("Transactions started : " + metricsService.TransactionStarts);
      TestContext.WriteLine("Transactions commited : " + metricsService.TransactionCommits);
      TestContext.WriteLine("Transactions rolled back : " + metricsService.TransactionRollbacks);

      TestContext.WriteLine("DynamicNonQueries ran : " + metricsService.DynamicNonQueries);
      TestContext.WriteLine("StoredProcedureNonQueries ran : " + metricsService.StoredProcedureNonQueries);
      TestContext.WriteLine("Readers ran : " + metricsService.Readers);
      TestContext.WriteLine("Readers total rows read : " + metricsService.ReaderRows);
      TestContext.WriteLine("Inserts ran : " + metricsService.Inserts);
      TestContext.WriteLine("Updates ran : " + metricsService.Updates);
      TestContext.WriteLine("Deletes ran : " + metricsService.Deletes);
      TestContext.WriteLine("SoftDeletes ran : " + metricsService.SoftDeletes);

      if (metricsService.Errors.Count != 0)
      {
        TestContext.WriteLine();
        foreach (var error in metricsService.Errors)
        {
          TestContext.WriteLine($"ERROR: {error.Message}");
        }
        TestContext.WriteLine();

        var dataMetricEvents = metricsService.GetDataMetricEvents();

        TestContext.WriteLine("===== Events =====");
        foreach (TestDataMetricEvent dataMetricEvent in dataMetricEvents)
        {
          TestContext.WriteLine(" ==== Event ====");
          TestContext.WriteLine(" Data metric type: " + dataMetricEvent.Type);

          if (dataMetricEvent.CommandType != null)
          {
            TestContext.WriteLine(" Command type: " + dataMetricEvent.CommandType);
          }
          if (dataMetricEvent.Stopwatch != null)
          {
            TestContext.WriteLine(" Elapsed time: " + dataMetricEvent.Stopwatch.Elapsed);
          }
          if (dataMetricEvent.Rows != null)
          {
            TestContext.WriteLine(" Affected rows : " + dataMetricEvent.Rows);
          }
          if (!string.IsNullOrEmpty(dataMetricEvent.CommandText))
          {
            TestContext.WriteLine(" Command: " + dataMetricEvent.CommandText.Trim('\r', '\n'));
          }

          if (dataMetricEvent.Parameters != null)
          {
            TestContext.WriteLine("  === Parameters ===");
            foreach (var parameter in dataMetricEvent.Parameters)
            {
              if (parameter.Value != null)
              {
                TestContext.WriteLine("  " + parameter.Key + " : " + parameter.Value);
              }
            }
          }

          if (dataMetricEvent.StackTrace != null)
          {
            TestContext.WriteLine("  === StackTrace ===");
            foreach (var stackTrace in dataMetricEvent.StackTrace)
            {
              TestContext.WriteLine(stackTrace);
            }
          }

          TestContext.WriteLine("");
        }
      }

      metricsService.Reset();
    }

    /// <summary>
    /// After each Scenario check if Session is still in transaction.
    /// </summary>
    [AfterScenario(Order = int.MinValue)]
    public void AfterMinScenario()
    {
      if (Session.IsTransaction())
      {
        Session.CommitTransaction();
      }
    }

    /// <summary>
    /// After each Scenario run registered CleanupActions and check if all errors are validated
    /// </summary>
    [AfterScenario(Order = int.MaxValue)]
    public void AfterMaxScenario()
    {
      DisposeSession();
      StartSession(SessionKey, PrivateKey);

      if (Context.TryGetValue<List<Action>>("CleanupActions", out var actions))
      {
        foreach (var action in actions.Reverse<Action>())
        {
          action();
        }

        Context.Remove("CleanupActions");
      }

      Assert.IsNull(ErrorMessage, "ErrorMessage has not been validated");
    }

    private int? GetLimit(string field, int? defaultValue)
    {
      string prefix = "Limit" + field + ":";

      foreach (var tag in Context.ScenarioInfo.Tags)
      {
        if (tag.StartsWith(prefix))
        {
          return int.Parse(tag.Substring(prefix.Length));
        }
      }

      return defaultValue;
    }
  }
}
