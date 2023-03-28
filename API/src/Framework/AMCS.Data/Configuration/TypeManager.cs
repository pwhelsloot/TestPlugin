using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if NETFRAMEWORK
using System.Web.Hosting;
#endif

namespace AMCS.Data.Configuration
{
  public class TypeManager : ITypeManager
  {
    public static TypeManager FromApplicationPath(params string[] assemblyPrefixes)
    {
      string directory = string.Empty;
#if NETFRAMEWORK
      if (HostingEnvironment.IsHosted)
        directory = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "bin");
      else
        directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#elif NETSTANDARD
      directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif

      if (!Directory.Exists(directory))
        throw new DirectoryNotFoundException("Could not determine the installation directory so cannot determine the known EntityObject types");

      var assemblies = new List<Assembly>();

      foreach (string path in Directory.GetFiles(directory, "*.dll"))
      {
        if (IsMatch(assemblyPrefixes, path))
          assemblies.Add(Assembly.Load(Path.GetFileNameWithoutExtension(path)));
      }

      return new TypeManager(assemblies.ToArray());
    }

    private static bool IsMatch(string[] assemblyPrefixes, string path)
    {
      if (assemblyPrefixes == null || assemblyPrefixes.Length == 0)
        return true;

      foreach (string assemblyPrefix in assemblyPrefixes)
      {
        if (Path.GetFileName(path).StartsWith(assemblyPrefix, StringComparison.OrdinalIgnoreCase))
          return true;
      }

      return false;
    }

    public static TypeManager FromFiles(IEnumerable<string> paths)
    {
      return new TypeManager(paths.Select(Assembly.LoadFile).ToArray());
    }

    public static TypeManager FromAssemblies(params Assembly[] assemblies)
    {
      return new TypeManager(assemblies);
    }

    public IList<Assembly> Assemblies { get; }

    private TypeManager(params Assembly[] assemblies)
    {
      Assemblies = new ReadOnlyCollection<Assembly>(assemblies.ToList());
    }

    public Type GetType(string typeName, bool throwOnError = true)
    {
      foreach (var assembly in Assemblies)
      {
        var type = assembly.GetType(typeName, false);
        if (type != null)
          return type;
      }

      if (throwOnError)
        throw new ArgumentException($"Cannot find type '{typeName}'", nameof(typeName));

      return null;
    }

    public IEnumerable<Type> GetTypes()
    {
      foreach (var assembly in Assemblies)
      {
        foreach (var type in assembly.GetTypes())
        {
          yield return type;
        }
      }
    }
  }
}
