namespace AMCS.ApiService
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;

  internal class UdfResultObject : IUdfResultObject
  {
    public int UdfFieldId { get; }

    public string FieldName { get; }

    public string Namespace { get; set; }

    public Guid RelatedResourceGuid { get; }

    public string StringValue { get; }

    public string TextValue { get; }

    public int? IntegerValue { get; }

    public decimal? DecimalValue { get; }

    public UdfResultObject(int udfFieldId, string fieldName, string @namespace, Guid relatedResourceGuid, string stringField, string textField, int? integerField, decimal? decimalField)
    {
      this.UdfFieldId = udfFieldId;
      this.FieldName = fieldName;
      this.Namespace = @namespace;
      this.RelatedResourceGuid = relatedResourceGuid;
      this.StringValue = stringField;
      this.TextValue = textField;
      this.IntegerValue = integerField;
      this.DecimalValue = decimalField;
    }
  }
}