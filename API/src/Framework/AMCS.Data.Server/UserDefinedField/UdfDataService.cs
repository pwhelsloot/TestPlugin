namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.Text;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;
  using Dapper;
  using Entity.UserDefinedField;
  using SQL;

  internal class UdfDataService : IUdfDataService
  {
    private const string CountQuery = @"
      WITH CountCTE AS (
        SELECT UdfFieldId FROM [ext].[UdfDataString] WHERE UdfFieldId = @UdfFieldId
        UNION
        SELECT UdfFieldId FROM [ext].[UdfDataText] WHERE UdfFieldId = @UdfFieldId
        UNION
        SELECT UdfFieldId FROM [ext].[UdfDataInteger] WHERE UdfFieldId = @UdfFieldId
        UNION
        SELECT UdfFieldId FROM [ext].[UdfDataDecimal] WHERE UdfFieldId = @UdfFieldId
      )
      SELECT COUNT(UdfFieldId) FROM CountCTE
    ";

    private readonly IUdfMetadataService udfMetadataService;
    
    public UdfDataService(IUdfMetadataService udfMetadataService)
    {
      this.udfMetadataService = udfMetadataService;
    }

    public Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(Guid relatedResourceGuid, IList<string> fieldNames, ISessionToken userId, IDataSession dataSession)
    {
      return Read(new List<Guid> { relatedResourceGuid }, fieldNames, GetAllAvailableNamespaces(), userId, dataSession);
    }

    public Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(Guid relatedResourceGuid, IList<string> fieldNames, IList<string> namespaces, ISessionToken userId,
      IDataSession dataSession)
    {
      return Read(new List<Guid> {relatedResourceGuid}, fieldNames, namespaces, userId, dataSession);
    }

    public Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(IList<Guid> relatedResourceGuids, IList<string> fieldNames, ISessionToken userId, IDataSession dataSession)
    {
      return Read(relatedResourceGuids, fieldNames, GetAllAvailableNamespaces(), userId, dataSession);
    }

    public Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(IList<Guid> relatedResourceGuids, IList<string> businessObjectNames, IList<string> namespaces, ISessionToken userId, IDataSession dataSession)
    {
      var queryBuilder = new StringBuilder();

      var parameters = new DynamicParameters();
      parameters.Add("@TenantId", userId.TenantId);
      parameters.Add("@Namespaces", namespaces);
      parameters.Add("@BusinessObjectNames", businessObjectNames);

      for (var i = 0; i < relatedResourceGuids.Count; i++)
      {
        parameters.Add($"@RelatedResourceGuid{i}", relatedResourceGuids[i]);
        queryBuilder.Append($@"
	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 0

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 5

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 6

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 7

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 8

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 9

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 10

	      UNION ALL

	      SELECT 
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], Value AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
	      FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataString] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 11

	      UNION ALL

	      SELECT
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], NULL AS [StringValue], Value AS [TextValue], NULL AS [IntegerValue], NULL AS [DecimalValue]
          FROM [ext].[UdfMetadata] AS [Metadata]	  
          LEFT JOIN [ext].[UdfDataText] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 1
	      
	      UNION ALL

	      SELECT
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], NULL AS [StringValue], NULL AS [TextValue], Value AS [IntegerValue], NULL AS [DecimalValue]
          FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataInteger] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 2
	      
	      UNION ALL

	      SELECT
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], NULL AS [StringValue], NULL AS [TextValue], Value AS [IntegerValue], NULL AS [DecimalValue]
          FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataInteger] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 3
	      
	      UNION ALL

	      SELECT
		      UdfMetadataId AS [UdfFieldId], BusinessObjectName, FieldName, Namespace, @RelatedResourceGuid{i} AS [RelatedResourceGuid], NULL AS [StringValue], NULL AS [TextValue], NULL AS [IntegerValue], Value AS [DecimalValue]
          FROM [ext].[UdfMetadata] AS [Metadata]
          LEFT JOIN [ext].[UdfDataDecimal] AS [Data] ON [Data].[UdfFieldId] = [Metadata].[UdfMetadataId]
            AND [RelatedResourceGuid] = @RelatedResourceGuid{i}
            AND [TenantId] = @TenantId
	      WHERE [Namespace] IN @Namespaces
            AND [BusinessObjectName] IN @BusinessObjectNames
            AND [Metadata].[DataType] = 4
        ");

        if (i < relatedResourceGuids.Count - 1)
          queryBuilder.Append("UNION ALL");
      }

      var results = dataSession.GetConnection().Query<UdfResultObject>(queryBuilder.ToString(), parameters, dataSession.GetTransaction())
        .ToArray();

      return results.Count() == 0 ? new Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>>() : FilterResultObjects(results);
    }

    public bool DataExistsFor(IDataSession dataSession, int udfFieldId)
    {
      var parameters = new DynamicParameters();
      parameters.Add("@UdfFieldId", udfFieldId);

      var query = dataSession.GetConnection().QuerySingleOrDefault<int?>(CountQuery, parameters, dataSession.GetTransaction());
      return query.HasValue && query > 0;
    }

    private IList<string> GetAllAvailableNamespaces()
    {
      var list = new List<string>();
      foreach (var @namespace in udfMetadataService.GetUdfMetadata().Namespaces)
      {
        list.Add(@namespace.Name);
      }

      return list;
    }
    
    private Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> FilterResultObjects(IEnumerable<UdfResultObject> resultObjects)
    {
      var udfNamespaces = udfMetadataService.GetUdfMetadata().Namespaces;
      var dict = new Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>>();

      foreach (var udfNamespace in udfNamespaces)
      {
        foreach (var udfField in udfNamespace.Fields)
        {
          foreach (var resultObject in resultObjects)
          {
            if (resultObject.FieldName != udfField.FieldName || resultObject.UdfFieldId != udfField.UdfFieldId)
              continue;

            if (dict.TryGetValue(resultObject.RelatedResourceGuid, out var existingInnerDictionary))
            {
              existingInnerDictionary.Add(udfField, resultObject);
            }
            else
            {
              dict[resultObject.RelatedResourceGuid] =
                new Dictionary<IUdfField, IUdfResultObject>
                {
                  { udfField, resultObject }
                };
            }
          }
        }
      }

      return dict;
    }

    public void Write(Guid relatedResourceGuid, Type entityType, IList<(string Namespace, Dictionary<string, object> Items)> values, ISessionToken userId, IDataSession dataSession)
    {
      DataServices.Resolve<IUdfValidationService>().Validate(values, relatedResourceGuid, entityType, out var results);

      foreach (var item in results)
      {
        Write(relatedResourceGuid, item.Key, item.Value, userId, dataSession);
      }
    }

    public void Delete(IList<Guid> relatedResourceGuids, ISessionToken userId, IDataSession dataSession)
    {
      if (relatedResourceGuids == null || relatedResourceGuids.Count <= 0)
        throw new InvalidOperationException("Must pass at least one resource GUID to Delete");

      var builder = new StringBuilder();

      builder.Append($"DELETE FROM [ext].[UdfDataString] WHERE [RelatedResourceGuid] IN (SELECT Guid FROM @RelatedResourceGuids);");
      builder.Append($"DELETE FROM [ext].[UdfDataInteger] WHERE [RelatedResourceGuid] IN (SELECT Guid FROM @RelatedResourceGuids);");
      builder.Append($"DELETE FROM [ext].[UdfDataDecimal] WHERE [RelatedResourceGuid] IN (SELECT Guid FROM @RelatedResourceGuids);");
      builder.Append($"DELETE FROM [ext].[UdfDataText] WHERE [RelatedResourceGuid] IN (SELECT Guid FROM @RelatedResourceGuids);");
      
      dataSession.Query(builder.ToString())
        .SetIdList("@RelatedResourceGuids", "RelatedResourceGuid", relatedResourceGuids, "dbo.GuidTableType")
        .ExecuteNonQuery();
    }

    private void Write(Guid relatedResourceGuid, IUdfField field, IUdfResultObject value, ISessionToken userId, IDataSession dataSession)
    {
      var mappedType = MapDataType(field.DataType);
      var tenantId = Guid.Parse(userId.TenantId);
      switch (mappedType)
      {
        case UdfTableType.Integer:
          UpsertToUdfInt(field.UdfFieldId, tenantId, relatedResourceGuid, true, value.IntegerValue, dataSession);
          break;

        case UdfTableType.Decimal:
          UpsertToUdfDecimal(field.UdfFieldId, tenantId, relatedResourceGuid, true, value.DecimalValue, dataSession);
          break;

        case UdfTableType.Text:
          UpsertToUdfStringOrText(field.UdfFieldId, tenantId, relatedResourceGuid, true, value.TextValue, true, dataSession);
          break;

        case UdfTableType.String:
          UpsertToUdfStringOrText(field.UdfFieldId, tenantId, relatedResourceGuid, true, value.StringValue, false, dataSession);
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(mappedType), $"Unknown UdfTableType of {mappedType}");
      }
    }

    private static void UpsertToUdfStringOrText(int udfFieldId, Guid tenantId, Guid relatedResourceGuid, bool isIndexed, string value, bool isLargeText, IDataSession dataSession)
    {
      var query = new StringBuilder();

      query.Append(isLargeText ? "MERGE [ext].[UdfDataText] AS [Target]" : "MERGE [ext].[UdfDataString] AS [Target]");
      query.Append(@"
        USING (SELECT [UdfFieldId] = @UdfFieldId, [TenantId] = @TenantId, [RelatedResourceGuid] = @RelatedResourceGuid) AS [Source]
          ON [Target].[UdfFieldId] = [Source].[UdfFieldId] 
            AND [Target].[TenantId] = [Source].[TenantId] 
            AND [Target].[RelatedResourceGuid] = [Source].[RelatedResourceGuid]
        WHEN MATCHED THEN
          UPDATE SET [Target].[Value] = @Value
        WHEN NOT MATCHED THEN
          INSERT (UdfFieldId, TenantId, RelatedResourceGuid, Value, IsIndexed)
            VALUES (@UdfFieldId, @TenantId, @RelatedResourceGuid, @Value, @IsIndexed);
      ");

      dataSession.Query(query.ToString())
        .Set("@Value", value)
        .Set("@UdfFieldId", udfFieldId)
        .Set("@TenantId", tenantId)
        .Set("@RelatedResourceGuid", relatedResourceGuid)
        .Set("@IsIndexed", isIndexed)
        .ExecuteNonQuery();
    }

    private static void UpsertToUdfInt(int udfFieldId, Guid tenantId, Guid relatedResourceGuid, bool isIndexed, int? value, IDataSession dataSession)
    {
      const string query = @"
        MERGE [ext].[UdfDataInteger] AS [Target]
        USING (SELECT [UdfFieldId] = @UdfFieldId, [TenantId] = @TenantId, [RelatedResourceGuid] = @RelatedResourceGuid) AS [Source]
          ON [Target].[UdfFieldId] = [Source].[UdfFieldId] 
            AND [Target].[TenantId] = [Source].[TenantId] 
            AND [Target].[RelatedResourceGuid] = [Source].[RelatedResourceGuid]
        WHEN MATCHED THEN
          UPDATE SET [Target].[Value] = @Value
        WHEN NOT MATCHED THEN
          INSERT (UdfFieldId, TenantId, RelatedResourceGuid, Value, IsIndexed)
            VALUES (@UdfFieldId, @TenantId, @RelatedResourceGuid, @Value, @IsIndexed);
      ";

      dataSession.Query(query)
        .Set("@Value", value)
        .Set("@UdfFieldId", udfFieldId)
        .Set("@TenantId", tenantId)
        .Set("@RelatedResourceGuid", relatedResourceGuid)
        .Set("@IsIndexed", isIndexed)
        .ExecuteNonQuery();
    }

    private static void UpsertToUdfDecimal(int udfFieldId, Guid tenantId, Guid relatedResourceGuid, bool isIndexed, decimal? value, IDataSession dataSession)
    {
      const string query = @"
        MERGE [ext].[UdfDataDecimal] AS [Target]
        USING (SELECT [UdfFieldId] = @UdfFieldId, [TenantId] = @TenantId, [RelatedResourceGuid] = @RelatedResourceGuid) AS [Source]
          ON [Target].[UdfFieldId] = [Source].[UdfFieldId] 
            AND [Target].[TenantId] = [Source].[TenantId] 
            AND [Target].[RelatedResourceGuid] = [Source].[RelatedResourceGuid]
        WHEN MATCHED THEN
          UPDATE SET [Target].[Value] = @Value
        WHEN NOT MATCHED THEN
          INSERT (UdfFieldId, TenantId, RelatedResourceGuid, Value, IsIndexed)
            VALUES (@UdfFieldId, @TenantId, @RelatedResourceGuid, @Value, @IsIndexed);
      ";

      dataSession.Query(query)
        .Set("@Value", value)
        .Set("@UdfFieldId", udfFieldId)
        .Set("@TenantId", tenantId)
        .Set("@RelatedResourceGuid", relatedResourceGuid)
        .Set("@IsIndexed", isIndexed)
        .ExecuteNonQuery();
    }
    
    private static UdfTableType MapDataType(DataType dataType)
    {
      switch (dataType)
      {
        case DataType.Integer:
        case DataType.Boolean:
          return UdfTableType.Integer;

        case DataType.Decimal:
          return UdfTableType.Decimal;

        case DataType.Text:
          return UdfTableType.Text;

        case DataType.String:
        case DataType.Duration:
        case DataType.LocalDate:
        case DataType.LocalDateTime:
        case DataType.LocalTime:
        case DataType.OffsetDateTime:
        case DataType.UtcDateTime:
        case DataType.ZonedDateTime:
          return UdfTableType.String;

        default:
          throw new ArgumentOutOfRangeException(nameof(dataType), $"Unknown data type of {dataType} when trying to map to UdfTableType");
      }
    }

    private enum UdfTableType
    {
      String,
      Text,
      Integer,
      Decimal
    }
  }
}