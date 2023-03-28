namespace AMCS.Data.Server.UserDefinedField
{
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  internal class UdfField : IUdfField
  {
    public int UdfFieldId { get; }
    
    public string BusinessObjectName { get; }
    
    public string FieldName { get; }

    public string Namespace { get; }

    public DataType DataType { get; }

    public bool Required { get; }

    public string Metadata { get; }

    public UdfField(int udfFieldId, string businessObjectName, string fieldName, DataType dataType, bool required,
      string @namespace, string metadata)
    {
      UdfFieldId = udfFieldId;
      BusinessObjectName = businessObjectName;
      FieldName = fieldName;
      DataType = dataType;
      Required = required;
      Metadata = metadata;
      Namespace = @namespace;
    }
  }
}