namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using Entity.UserDefinedField;

  internal class UdfResultObject : IUdfResultObject
  {
    public int UdfFieldId { get; }

    public string FieldName { get; }

    public string Namespace { get; }

    public Guid RelatedResourceGuid { get; }

    public string StringValue { get; }
    
    public string TextValue { get; }
    
    public int? IntegerValue { get; }
    
    public decimal? DecimalValue { get; }

    public UdfResultObject()
    {
    }
    
    public UdfResultObject(int udfFieldId, string fieldName, string @namespace, Guid relatedResourceGuid, string stringValue, string textValue, int? integerValue, decimal? decimalValue)
    {
      this.UdfFieldId = udfFieldId;
      this.FieldName = fieldName;
      this.Namespace = @namespace;
      this.RelatedResourceGuid = relatedResourceGuid;
      this.StringValue = stringValue;
      this.TextValue = textValue;
      this.IntegerValue = integerValue;
      this.DecimalValue = decimalValue;
    }
  }
}