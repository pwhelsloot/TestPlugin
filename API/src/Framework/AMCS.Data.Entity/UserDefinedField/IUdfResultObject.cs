namespace AMCS.Data.Entity.UserDefinedField
{
  using System;

  public interface IUdfResultObject
  {
    int UdfFieldId { get; }

    string FieldName { get; }

    string Namespace { get; }

    Guid RelatedResourceGuid { get; }
    
    string StringValue { get; }
    
    string TextValue { get; }
    
    int? IntegerValue { get; }
    
    decimal? DecimalValue { get; }
  }
}