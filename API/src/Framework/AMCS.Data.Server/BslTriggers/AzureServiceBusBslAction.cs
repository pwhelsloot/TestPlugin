namespace AMCS.Data.Server.BslTriggers
{
  using System;
  using System.Runtime.InteropServices;
  using System.Text;
  using AMCS.Data.Server.AzureServiceBus;
  using log4net;
  using Newtonsoft.Json;
  using global::Azure.Messaging.ServiceBus;

  [Guid("7CD19F93-7401-47B0-8D7C-E379895FE97F")]
  public class AzureServiceBusBslAction : IBslAction<AzureServiceBusBslActionConfiguration>
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureServiceBusBslAction));

    public void Execute(ISessionToken userId, BslTriggerRequest request, AzureServiceBusBslActionConfiguration configuration, IBslActionContext bslActionContext)
    {
      try
      {
        DataServices.Resolve<IAzureServiceBusConnectionManager>().SendMessage(configuration, CreateMessage(request));
      }
      catch (Exception ex)
      {
        Logger.Error(ex);
      }
    }

    private ServiceBusMessage CreateMessage(BslTriggerRequest request)
    {
      var json = JsonConvert.SerializeObject(new 
      {
        Entity = GetEntityName(request.EntityType),
        Id = request.Id, 
        Guid = request.GUID, 
        Action = request.Action.ToString(),
      });

      return new ServiceBusMessage(Encoding.UTF8.GetBytes(json));
    }

    private string GetEntityName(string entityType)
    {
      var entityName = entityType.Substring(0, entityType.IndexOf(','));
      entityName = entityName.Substring(entityName.LastIndexOf('.') + 1);
      return entityName.EndsWith("Entity") 
        ? entityName.Substring(0, entityName.LastIndexOf("Entity", StringComparison.OrdinalIgnoreCase)) 
        : entityName;
    }
  }
}
