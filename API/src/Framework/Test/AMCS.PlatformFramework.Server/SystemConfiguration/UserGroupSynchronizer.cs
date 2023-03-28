using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.SystemConfiguration;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.SystemConfiguration
{
  internal class UserGroupSynchronizer : IConfigurationSynchronizer<UserGroup>
  {
    public IList<UserGroup> GetAll(ISessionToken userId, IDataSession dataSession)
    {
      var result = new List<UserGroup>();

      foreach (var entity in dataSession.GetAll<UserGroupEntity>(userId, false))
      {
        result.Add(new UserGroup
        {
          Name = entity.Name,
          IsAdministrator = entity.IsAdministrator
        });
      }

      return result;
    }

    public void Save(ISessionToken userId, IList<UserGroup> elements, Transform transform, IDataSession dataSession)
    {
      var userGroups = dataSession
        .GetAll<UserGroupEntity>(userId, false)
        .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

      var seen = new HashSet<UserGroupEntity>();

      foreach (var element in elements)
      {
        try
        {
          if (userGroups.TryGetValue(element.Name, out var userGroup))
          {
            seen.Add(userGroup);
          }
          else
          {
            userGroup = new UserGroupEntity
            {
              Name = element.Name
            };
          }

          userGroup.IsAdministrator = element.IsAdministrator;

          dataSession.Save(userId, userGroup);
        }
        catch (Exception ex)
        {
          element.ImportError = ex.Message;
        }
      }

      if (transform == Transform.Default)
        transform = Transform.Replace;

      if (transform == Transform.Replace)
      {
        foreach (var userGroup in userGroups.Values.Where(p => !seen.Contains(p)))
        {
          dataSession.Delete(userId, userGroup, false);
        }
      }
    }
    
    public IList<string> Validate(IList<UserGroup> elements)
    {
      return new List<string>();
    }
  }
}
