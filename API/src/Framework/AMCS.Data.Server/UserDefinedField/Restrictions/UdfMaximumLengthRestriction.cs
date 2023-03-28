namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public class UdfMaximumLengthRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      var maxLength = Convert.ToDecimal(UdfJsonUtil.GetSingleUdfRestrictionValue(metadata));
      switch (udfField.DataType)
      {
        case DataType.String:
          if (resultObject.StringValue.Length > maxLength)
            throw new UdfValidationException($"Maximum length exceeded for string on {udfField.Namespace}.{udfField.FieldName}");
          break;

        case DataType.Text:
          if (resultObject.TextValue.Length > maxLength)
            throw new UdfValidationException($"Maximum length exceeded for text on {udfField.Namespace}.{udfField.FieldName}");
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfMaximumLengthRestriction");
      }
    }
  }
}