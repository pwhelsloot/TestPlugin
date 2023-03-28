using AMCS.Data.Configuration;
using AMCS.Data.EndToEnd.Tests.DataMetrics;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests
{
  public static class DataConfigurationExtensions
  {
    public static void ConfigureDataMetricsEvents(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterInstance(new TestDataMetricsEventsService())
        .AsSelf()
        .As<ITestDataMetricsEventsService>();

      self.ContainerBuilder
        .RegisterType<TestMetricsEventsRegistrationService>()
        .SingleInstance()
        .AutoActivate();
    }

    private class TestMetricsEventsRegistrationService
    {
      public TestMetricsEventsRegistrationService(
        IDataMetricsEventsBuilderService dataMetricsEventsBuilderService,
        ITestDataMetricsEventsService sampleDataMetricsEventsService)
      {
        dataMetricsEventsBuilderService.Add(new TestDataMetricsEvents(sampleDataMetricsEventsService));
      }
    }
  }
}
