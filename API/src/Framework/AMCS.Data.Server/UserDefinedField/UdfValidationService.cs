namespace AMCS.Data.Server.UserDefinedField
{
  using System.Collections.Generic;
  using Newtonsoft.Json;
  using System;
  using System.IO;
  using System.Text;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.Data.Server.UserDefinedField.Restrictions;
  using WriteValidations;
  using DataType = PluginData.Data.Metadata.UserDefinedFields.DataType;

  internal class UdfValidationService : IUdfValidationService
  {
    private readonly IUdfMetadataService udfMetadataService;
    private readonly IEnumerable<IUdfWriteValidation> writeValidations;

    public UdfValidationService(IUdfMetadataService udfMetadataService, IEnumerable<IUdfWriteValidation> writeValidations)
    {
      this.udfMetadataService = udfMetadataService;
      this.writeValidations = writeValidations;
    }

    public void Validate(IList<(string Namespace, Dictionary<string, object> Items)> udfValuesToWrite, Guid relatedResourceGuid, Type entityType, out Dictionary<IUdfField, IUdfResultObject> results)
    {
      var udfWriteValidationParameters = new UdfWriteValidationParameters
      {
        UdfValues = udfValuesToWrite,
        EntityType = entityType
      };

      var errorMessages = new StringBuilder("The following errors were found when validating UDF metadata: [");
      var foundErrors = false;
      foreach (var writeValidation in writeValidations)
      {
        if (!writeValidation.Validate(udfWriteValidationParameters, out var errorMessage))
        {
          errorMessages.Append("'");
          errorMessages.Append(errorMessage);
          errorMessages.Append("'");
          errorMessages.Append(",");
          foundErrors = true;
        }
      }

      if (foundErrors)
      {
        errorMessages = errorMessages.Remove(errorMessages.Length - 1, 1);
        errorMessages.Append("]");
        throw new UdfValidationException(errorMessages.ToString());
      }
      
      results = new Dictionary<IUdfField, IUdfResultObject>();

      var namespaces = udfMetadataService.GetUdfMetadata().Namespaces;

      foreach (var incomingUdfSet in udfValuesToWrite)
      {
        foreach (var @namespace in namespaces)
        {
          if (incomingUdfSet.Namespace != @namespace.Name)
            continue;

          foreach (var incomingUdfValue in incomingUdfSet.Items)
          {
            foreach (var udfField in @namespace.Fields)
            {
              if (incomingUdfValue.Key != udfField.FieldName)
                continue;

              results.Add(udfField, GenerateResultObject(udfField, relatedResourceGuid, incomingUdfValue.Value));
            }
          }
        }
      }

      foreach (var result in results)
      {
        if (string.IsNullOrWhiteSpace(result.Key.Metadata))
          continue;

        ExecuteUdfRestrictions(result.Key, result.Value, result.Key.Metadata);
      }
    }
    
    private void ExecuteUdfRestrictions(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      using (var stringReader = new StringReader(metadata))
      using (var jsonReader = new JsonTextReader(stringReader))
      {
        var startedObject = false;
        var endedObject = false;
        var foundRestrictionName = false;
        string currentRestrictionName = null;
        var valueSet = false;
        var restrictionObject = new StringBuilder();

        while (jsonReader.Read())
        {
          if (jsonReader.TokenType == JsonToken.StartObject)
          {
            restrictionObject.Append("{");
            startedObject = true;
          }

          if (startedObject)
          {
            switch (jsonReader.TokenType)
            {
              case JsonToken.PropertyName when jsonReader.Value != null:
              {
                if (string.Equals(jsonReader.Value.ToString(), "Name", StringComparison.CurrentCultureIgnoreCase))
                  foundRestrictionName = true;

                restrictionObject.Append(valueSet ? $",\"{jsonReader.Value}\":" : $"\"{jsonReader.Value}\":");
                break;
              }
              case JsonToken.String:
              case JsonToken.Date:
              {
                if (foundRestrictionName)
                {
                  currentRestrictionName = jsonReader.Value.ToString();
                  foundRestrictionName = false;
                }

                valueSet = true;

                restrictionObject.Append($"\"{jsonReader.Value}\"");
                break;
              }
              case JsonToken.Boolean:
              case JsonToken.Float:
              case JsonToken.Integer:
                valueSet = true;
                restrictionObject.Append(jsonReader.Value);
                break;
              case JsonToken.EndObject:
                restrictionObject.Append("}");
                startedObject = false;
                endedObject = true;
                valueSet = false;
                break;
              default:
                restrictionObject.Append(jsonReader.Value);
                break;
            }
          }

          if (endedObject)
          {
            DataServices.TryResolveKeyed<IUdfRestriction>(currentRestrictionName, out var restriction);
            restriction.Execute(udfField, resultObject, restrictionObject.ToString());
            restrictionObject.Clear();
            currentRestrictionName = null;
            endedObject = false;
          }
        }
      }
    }
    
    private IUdfResultObject GenerateResultObject(IUdfField udfField, Guid entityGuid, object value)
    {
      switch (udfField.DataType)
      {
        case DataType.Boolean:
        case DataType.Integer:
          return new UdfResultObject(udfField.UdfFieldId, udfField.FieldName, udfField.Namespace, entityGuid, null,
            null, Convert.ToInt32(value), null);
        case DataType.Decimal:
          return new UdfResultObject(udfField.UdfFieldId, udfField.FieldName, udfField.Namespace, entityGuid, null,
            null, null, Convert.ToDecimal(value));
        case DataType.Text:
          return new UdfResultObject(udfField.UdfFieldId, udfField.FieldName, udfField.Namespace, entityGuid, null,
            Convert.ToString(value), null, null);
        case DataType.String:
        case DataType.Duration:
        case DataType.LocalDate:
        case DataType.LocalTime:
        case DataType.LocalDateTime:
        case DataType.OffsetDateTime:
        case DataType.ZonedDateTime:
        case DataType.UtcDateTime:
          return new UdfResultObject(udfField.UdfFieldId, udfField.FieldName, udfField.Namespace, entityGuid, Convert.ToString(value),
            null, null, null);
        default:
          throw new ArgumentOutOfRangeException(nameof(udfField.DataType));
      }
    }
  }
}