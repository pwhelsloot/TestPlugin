using AMCS.Data.Mocking;
using AMCS.Data.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.TestSupport
{
  public static class MockDataServicesExtensions
  {
    public static MockDataServices AddEntityObjectService<T>(this MockDataServices self, T service) 
      where T : IEntityObjectService
    {
      return AddByInterface(self, service, typeof(IEntityObjectService));
    }

    public static MockDataServices AddEntityObjectAccess<T>(this MockDataServices self, T service) 
      where T : IEntityObjectAccess
    {
      return AddByInterface(self, service, typeof(IEntityObjectAccess));
    }

    public static MockDataServices AddByInterface(this MockDataServices self, object service, Type limit)
    {
      return self.Add(GetInterfaces(service.GetType(), limit), service);
    }

    private static Type[] GetInterfaces(Type type, Type limit)
    {
      return type.GetInterfaces()
        .Where(p => p != limit && limit.IsAssignableFromGeneric(p))
        .ToArray();
    }
  }
}
