namespace AMCS.Data.Server.Plugin
{
  using System;
  using AMCS.Data.Server.Services;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.Data.Server.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;

  internal class UdfMetadataMetadataProcessor : IUdfMetadataMetadataProcessor
  {
    private readonly IUdfMetadataCacheService udfMetadataCacheService;
    private readonly IBusinessObjectService businessObjectService;

    public UdfMetadataMetadataProcessor(IUdfMetadataCacheService udfMetadataCacheService, IBusinessObjectService businessObjectService)
    {
      this.udfMetadataCacheService = udfMetadataCacheService;
      this.businessObjectService = businessObjectService;
    }

    public void Process(IList<PluginData.Data.Metadata.UserDefinedFields.UdfMetadata> udfMetadatas, string fullyQualifiedPluginName, ISessionToken sessionToken, IDataSession dataSession)
    {
      var existingUdfMetadataEntities = dataSession.GetAllByTemplate(sessionToken,
        new UdfMetadataEntity { Namespace = fullyQualifiedPluginName }, true);
      var businessObjects = businessObjectService.GetAll();
      var udfMetadataEntities = new List<UdfMetadataEntity>();
      
      foreach (var udfMetadata in udfMetadatas)
      {
        if (businessObjects.All(businessObject => businessObject.BusinessObject.Name != udfMetadata.GetSanitizedName()))
        {
          throw new InvalidOperationException(
            $"Error while processing user defined field metadata for {udfMetadata.GetSanitizedName()}; Item not specified as a business object");
        }

        if (businessObjects
              .SingleOrDefault(businessObject => businessObject.BusinessObject.Name == udfMetadata.GetSanitizedName())
              ?.BusinessObject.AllowUserDefinedFields == false)
        {
          throw new InvalidOperationException($"Business object {udfMetadata.GetSanitizedName()} does not allow user defined fields");
        }
        
        var metadata = udfMetadata.Restrictions.Count > 0
          ? JsonConvert.SerializeObject(udfMetadata.Restrictions, new StringEnumConverter())
          : null;
       
        var udfMetadataEntity = new UdfMetadataEntity
        {
          BusinessObjectName = udfMetadata.GetSanitizedName(),
          Namespace = fullyQualifiedPluginName,
          FieldName = udfMetadata.FieldName,
          DataType = (int)udfMetadata.DataType,
          Required = udfMetadata.Required,
          Metadata = metadata
        };

        var existingUdfMetadata = existingUdfMetadataEntities.SingleOrDefault(udf =>
          udf.BusinessObjectName == udfMetadataEntity.BusinessObjectName &&
          udf.Namespace == udfMetadataEntity.Namespace &&
          udf.FieldName == udfMetadataEntity.FieldName);

        if (existingUdfMetadata != null && existingUdfMetadata.DataType != udfMetadataEntity.DataType)
        {
          throw new InvalidOperationException(
            $"Udf does not support changing the data type for existing UDF fields; Expected: {(DataType)existingUdfMetadata.DataType}, Actual: {udfMetadata.DataType} for {fullyQualifiedPluginName}.{udfMetadata.FieldName}");
        }

        if (existingUdfMetadata?.IsDeletePending == true)
          continue;

        udfMetadataEntity.UdfMetadataId = existingUdfMetadata?.Id32;
        udfMetadataEntities.Add(udfMetadataEntity);
      }

      udfMetadataCacheService.Publish(udfMetadataEntities, fullyQualifiedPluginName,
        sessionToken, dataSession);
    }
  }
}