namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.Collections.Generic;
  using AMCS.Data.Entity.UserDefinedField;

  internal class UdfMetadataService : EntityObjectService<UdfMetadataEntity>, IUdfMetadataService
  {
    private readonly IUdfMetadataCacheService udfMetadataCacheService;

    public UdfMetadataService(IUdfMetadataCacheService udfMetadataCacheService, IEntityObjectAccess<UdfMetadataEntity> dataAccess)
      : base(dataAccess)
    {
      this.udfMetadataCacheService = udfMetadataCacheService;
    }

    public IUdfMetadata GetUdfMetadata() => udfMetadataCacheService.GetUdfMetadata();
    
    public void SaveUdfMetadata(IList<UdfMetadataEntity> udfMetadata, string category, ISessionToken sessionToken, IDataSession dataSession)
    {
      udfMetadataCacheService.Publish(udfMetadata, category, sessionToken, dataSession);
    }

    public bool IsTypeValidForUdfMetadata(string @namespace, string fieldName, Type specifiedType)
    {
      var cachedLinks = udfMetadataCacheService.GetLinkedBusinessObjectTypes();
      return cachedLinks.TryGetValue((@namespace, fieldName), out var types) && types.Contains(specifiedType);
    }

    public override void Delete(ISessionToken userId, UdfMetadataEntity entity, IDataSession existingDataSession = null)
    {
      // If there's data associated with the metadata entity, mark it as pending and let job system job take care of deleting
      // udf data. Otherwise, delete as usual
      var dataService = DataServices.Resolve<IUdfDataService>();
      if (dataService.DataExistsFor(existingDataSession, entity.UdfMetadataId.Value))
      {
        entity.IsDeletePending = true;
        Save(userId, entity, existingDataSession);
      }
      else
      {
        base.Delete(userId, entity, existingDataSession);
      }
    }
  }
}