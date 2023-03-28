using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.SystemConfiguration;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.SystemConfiguration
{
  internal class SystemConfigurationSynchronizer : IConfigurationSynchronizer<SystemConfiguration>
  {
    public IList<SystemConfiguration> GetAll(ISessionToken userId, IDataSession dataSession)
    {
      var result = new List<SystemConfiguration>();

      foreach (var entity in dataSession.GetAll<SystemConfigurationEntity>(userId, false))
      {
        result.Add(new SystemConfiguration
        {
          Name = entity.Name,
          Value = entity.Value
        });
      }

      return result;
    }

    public void Save(ISessionToken userId, IList<SystemConfiguration> elements, Transform transform, IDataSession dataSession)
    {
      if (transform == Transform.Replace)
        throw new InvalidOperationException("System configuration parameters cannot be removed; invalid transform mode");

      var entities = dataSession
        .GetAll<SystemConfigurationEntity>(userId, false)
        .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

      foreach (var element in elements)
      {
        try
        {
          if (!entities.TryGetValue(element.Name, out var entity))
            throw new InvalidOperationException($"No system configuration exists by name '{element.Name}'");

          entity.Value = element.Value;

          dataSession.Save(userId, entity);
        }
        catch (Exception ex)
        {
          element.ImportError = ex.Message;
        }
      }
    }
    
    public IList<string> Validate(IList<SystemConfiguration> elements)
    {
      return new List<string>();
    }
  }
}
