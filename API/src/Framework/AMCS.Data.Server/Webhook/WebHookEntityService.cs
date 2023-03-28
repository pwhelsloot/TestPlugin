namespace AMCS.Data.Server.WebHook
{
  using AMCS.Data;
  using Data.Configuration;
  using Entity.WebHook;

  public class WebHookEntityService : EntityObjectService<WebHookEntity>
  {
    private readonly IServiceRootResolver serviceRootResolver;

    public WebHookEntityService(IEntityObjectAccess<WebHookEntity> dataAccess, IServiceRootResolver serviceRootResolver)
      : base(dataAccess)
    {
      this.serviceRootResolver = serviceRootResolver;
    }

    public override int? Save(ISessionToken userId, WebHookEntity entity, IDataSession existingDataSession = null)
    {
      // unconditionally override Environment with the Service Root
      entity.Environment = serviceRootResolver.ServiceRoot;

      return base.Save(userId, entity, existingDataSession);
    }
  }
}