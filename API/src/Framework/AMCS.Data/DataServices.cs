using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Mocking;
using Autofac;

namespace AMCS.Data
{
  public static class DataServices
  {
    private static volatile IContainer Container;
    [ThreadStatic]
    private static IMockContainer MockContainer;

    internal static void SetServices(IContainer services)
    {
      if (Container != null)
        throw new InvalidOperationException("Services already initialized");
      Container = services;
    }

    internal static void SetMockServices(IMockContainer mockContainer)
    {
      if (Container != null)
        throw new InvalidOperationException("DataServices has already been initialized; cannot activate mock services");
      MockContainer = mockContainer;
    }

    [DebuggerStepThrough]
    public static object Resolve(Type type)
    {
      var container = Container;
      if (container != null)
        return container.Resolve(type);
      var mockContainer = MockContainer;
      if (mockContainer != null)
        return mockContainer.Resolve(type);
      throw new InvalidOperationException("DataServices has not been setup");
    }

    [DebuggerStepThrough]
    public static T Resolve<T>()
    {
      var container = Container;
      if (container != null)
        return container.Resolve<T>();
      var mockContainer = MockContainer;
      if (mockContainer != null)
        return (T)mockContainer.Resolve(typeof(T));
      throw new InvalidOperationException("DataServices has not been setup");
    }

    [DebuggerStepThrough]
    public static T ResolveKeyed<T>(object key)
    {
      var container = Container;
      if (container != null)
        return container.ResolveKeyed<T>(key);

      throw new InvalidOperationException("DataServices has not been setup");
    }

    [DebuggerStepThrough]
    public static bool TryResolveKeyed(object key, Type type, out object instance)
    {
      var container = Container;
      if (container != null)
        return container.TryResolveKeyed(key, type, out instance);

      throw new InvalidOperationException("DataServices has not been setup");
    }

    [DebuggerStepThrough]
    public static bool TryResolveKeyed<T>(object key, out T instance)
    {
      var container = Container;
      if (container != null)
      {
        var result = container.TryResolveKeyed(key, typeof(T), out var tryInstance);
        instance = result ? (T)tryInstance : default;

        return result;
      }

      throw new InvalidOperationException("DataServices has not been setup");
    }

    [DebuggerStepThrough]
    public static bool TryResolve(Type type, out object instance)
    {
      var container = Container;
      if (container != null)
        return container.TryResolve(type, out instance);
      var mockContainer = MockContainer;
      if (mockContainer != null)
        return mockContainer.TryResolve(type, out instance);
      throw new InvalidOperationException("DataServices has not been setup");
    }

    [DebuggerStepThrough]
    public static bool TryResolve<T>(out T instance)
    {
      var container = Container;
      if (container != null)
        return container.TryResolve<T>(out instance);
      var mockContainer = MockContainer;
      if (mockContainer != null)
      {
        if (mockContainer.TryResolve(typeof(T), out var obj))
        {
          instance = (T)obj;
          return true;
        }
        instance = default(T);
        return false;
      }
      throw new InvalidOperationException("DataServices has not been setup");
    }
  }
}
