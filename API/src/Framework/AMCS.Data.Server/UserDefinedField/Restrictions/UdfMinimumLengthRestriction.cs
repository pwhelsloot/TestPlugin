namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public class UdfMinimumLengthRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      var minLength = Convert.ToDecimal(UdfJsonUtil.GetSingleUdfRestrictionValue(metadata));
      switch (udfField.DataType)
      {
        case DataType.String:
          if (minLength > resultObject.StringValue.Length)
            throw new UdfValidationException($"Minimum length not met for string on {udfField.Namespace}.{udfField.FieldName}");
          break;

        case DataType.Text:
          if (minLength > resultObject.TextValue.Length)
            throw new UdfValidationException($"Minimum length not met for text on {udfField.Namespace}.{udfField.FieldName}");
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfMinimumLengthRestriction");
      }
    }
  }
}