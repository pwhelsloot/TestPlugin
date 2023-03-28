using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data
{
  public static class TypeExtensions
  {
    public static bool IsAssignableFromGeneric(this Type self, Type type)
    {
      if (!self.IsGenericTypeDefinition)
        return self.IsAssignableFrom(type);

      while (type != null)
      {
        if (self.IsInterface)
        {
          foreach (var interfaceType in type.GetInterfaces())
          {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == self)
              return true;
          }
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == self)
          return true;

        type = type.BaseType;
      }

      return false;
    }

    public static bool CanConstruct(this Type self)
    {
      return
        self.IsClass &&
        !self.IsAbstract &&
        self.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null) != null;
    }

    public static Type[] GetGenericTypeArguments(this Type self, Type targetType)
    {
      while (self != null)
      {
        if (targetType.IsInterface)
        {
          foreach (var interfaceType in self.GetInterfaces())
          {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == targetType)
              return interfaceType.GenericTypeArguments;
          }
        }

        if (self.IsGenericType && self.GetGenericTypeDefinition() == targetType)
          return self.GenericTypeArguments;

        self = self.BaseType;
      }

      return null;
    }
  }
}
