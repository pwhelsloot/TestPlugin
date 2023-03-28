using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace AMCS.Data.Configuration
{
  public class DataConfiguration : IDisposable
  {
    private IContainer container;
    private bool disposed;
    private readonly SetupService setupService;
    public ContainerBuilder ContainerBuilder { get; } = new ContainerBuilder();
    public DataConfiguration()
    {
      this.setupService = new SetupService();
      ContainerBuilder.RegisterInstance(this.setupService)
        .As<ISetupService>();
    }
    public void Build()
    {
      container = ContainerBuilder.Build();
      DataServices.SetServices(container);
      setupService.RegisterSetup(Setup);
      setupService.RaiseSetup();
    }
    private void Setup()
    {
      var types = container.ComponentRegistry
        .Registrations
        .Where(p => typeof(IDelayedStartup).IsAssignableFrom(p.Activator.LimitType))
        .Select(p => p.Activator.LimitType);
      foreach (var type in types)
      {
        ((IDelayedStartup)container.Resolve(type)).Start();
      }
    }
    public void Dispose()
    {
      if (!disposed)
      {
        if (container != null)
        {
          container.Dispose();
          container = null;
        }
        disposed = true;
      }
    }
    private class SetupService : ISetupService
    {
      private readonly OrderedCallbacks setupCallbacks = new OrderedCallbacks();
      public void RaiseSetup()
      {
        setupCallbacks.Raise();
      }
      public void RegisterSetup(Action callback, int order = 0)
      {
        setupCallbacks.Register(callback, order);
      }
    }
  }
}