namespace AMCS.Data.Entity.UserDefinedField
{
  using System;
  using System.Runtime.Serialization;
  using Udf = PluginData.Data.Metadata.UserDefinedFields;

  [EntityTable("UdfMetadata", "UdfMetadataId", SchemaName = "ext")]
  public class UdfMetadataEntity : CacheCoherentEntity
  {
    [EntityMember]
    public int? UdfMetadataId { get; set; }

    [EntityMember]
    public string BusinessObjectName { get; set; }

    [EntityMember]
    public string Namespace { get; set; }

    [EntityMember]
    public string FieldName { get; set; }

    [EntityMember]
    public int DataType { get; set; }

    [EntityMember]
    public bool Required { get; set; }

    [EntityMember]
    public string Metadata { get; set; }

    [EntityMember]
    public bool IsDeletePending { get; set; }

    public override int? GetId() => UdfMetadataId;

    private static readonly string[] ValidatedProperties =
    {
      nameof(BusinessObjectName),
      nameof(FieldName),
      nameof(DataType)
    };

    public override string[] GetValidatedProperties() => ValidatedProperties;

    protected override string GetValidationError(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(BusinessObjectName):
          if (string.IsNullOrWhiteSpace(BusinessObjectName))
            return "Business object name cannot be empty";
          break;

        case nameof(FieldName):
          if (string.IsNullOrWhiteSpace(FieldName))
            return "Field name cannot be empty";
          break;

        case nameof(DataType):
          if (!Enum.TryParse<Udf.DataType>(DataType.ToString(), out _))
            return "Invalid data type";
          break;
      }

      return null;
    }
  }
}