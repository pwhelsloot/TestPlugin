namespace AMCS.Data.Entity.UserDefinedField
{
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public interface IUdfField
  {
    int UdfFieldId { get; }
    
    string BusinessObjectName { get; }

    string FieldName { get; }

    string Namespace { get; }

    DataType DataType { get; }

    bool Required { get; }

    string Metadata { get; }
  }
}