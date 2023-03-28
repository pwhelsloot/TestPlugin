namespace AMCS.Data.Server.Webhook
{
  using System;
  using System.Threading.Tasks;
  using PluginData.Data.WebHook;

  public interface IWebHookManager
  {
    Task RaiseSaveAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null);

    Task RaiseInsertAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null);

    Task RaiseUpdateAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null);

    Task RaiseDeleteAsync(ISessionToken userId, string businessObjectName, Guid entityGuid, IDataSession existingDataSession = null);
  }
}