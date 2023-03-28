namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public class UdfMinimumValueRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      var minValue = Convert.ToDecimal(UdfJsonUtil.GetSingleUdfRestrictionValue(metadata));
      switch (udfField.DataType)
      {
        case DataType.Integer:
          if (minValue > resultObject.IntegerValue)
            throw new UdfValidationException($"Minimum value not met for integer on {udfField.Namespace}.{udfField.FieldName}");
          break;

        case DataType.Decimal:
          if (minValue > resultObject.DecimalValue)
            throw new UdfValidationException($"Minimum value not met for decimal on {udfField.Namespace}.{udfField.FieldName}");
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfMinimumValueRestriction");
      }
    }
  }
}