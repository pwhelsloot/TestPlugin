using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;

namespace AMCS.Data.Mocking
{
  public class MockDataServices
  {
    private readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();
    private bool activated;

    private void VerifyNotActivated()
    {
      if (activated)
      {
        throw new InvalidOperationException("Mock data services cannot be modified after it has been activated");
      }
    }

    public MockDataServices Add(object service) => Add(service.GetType(), service);

    public MockDataServices Add(Type type, object service) => Add(new Type[] { type }, service);

    public MockDataServices Add(Type[] types, object service)
    {
      VerifyNotActivated();

      var singleton = new SingletonService(service);
      foreach (Type type in types)
      {
        services.Add(type, singleton);
      }

      return this;
    }

    public MockDataServices AddFactory<T>(Func<T> factory)
    {
      VerifyNotActivated();
      services.Add(typeof(T), new FactoryService<T>(factory));
      return this;
    }

    public IDisposable Activate()
    {
      VerifyNotActivated();

      activated = true;

      DataServices.SetMockServices(new MockContainer(services));

      return new Finalizer(() => DataServices.SetMockServices(null));
    }

    private interface IService
    {
      object GetService();
    }

    private class SingletonService : IService
    {
      private readonly object service;

      public SingletonService(object service)
      {
        this.service = service;
      }

      public object GetService()
      {
        return service;
      }
    }

    private class FactoryService<T> : IService
    {
      private readonly Func<T> factory;

      public FactoryService(Func<T> factory)
      {
        this.factory = factory;
      }

      public object GetService()
      {
        return factory();
      }
    }

    private class MockContainer : IMockContainer
    {
      private readonly Dictionary<Type, IService> services;

      public MockContainer(Dictionary<Type, IService> services)
      {
        this.services = services;
      }

      public object Resolve(Type type)
      {
        if (services.TryGetValue(type, out var service))
        {
          return service.GetService();
        }

        throw new MockDataServicesException($"Cannot resolve service of type '{type}' because it has not been registered");
      }

      public bool TryResolve(Type type, out object instance)
      {
        if (services.TryGetValue(type, out var service))
        {
          instance = service.GetService();
          return true;
        }

        instance = null;
        return false;
      }
    }
  }
}