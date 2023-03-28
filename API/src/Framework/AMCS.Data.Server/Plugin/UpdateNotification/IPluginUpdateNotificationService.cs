namespace AMCS.Data.Server.Plugin.UpdateNotification
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Entity.Tenants;

  public interface IPluginUpdateNotificationService
  {
    ConcurrentDictionary<string, Exception> TenantUpdates { get; }
    
    List<Task<bool>> Update(bool force = false);
    List<Task<bool>> Update(ITenant tenant, bool force = false);
    List<Task<bool>> Update(IList<ITenant> tenants, bool force = false);
  }
}