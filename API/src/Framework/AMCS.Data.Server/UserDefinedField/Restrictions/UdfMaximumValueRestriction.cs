namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public class UdfMaximumValueRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      var maxValue = Convert.ToDecimal(UdfJsonUtil.GetSingleUdfRestrictionValue(metadata));
      switch (udfField.DataType)
      {
        case DataType.Integer:
          if (resultObject.IntegerValue > maxValue)
            throw new UdfValidationException($"Maximum value exceeded for integer on {udfField.Namespace}.{udfField.FieldName}");
          break;

        case DataType.Decimal:
          if (resultObject.DecimalValue > maxValue)
            throw new UdfValidationException($"Maximum value exceeded for decimal on {udfField.Namespace}.{udfField.FieldName}");
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfMaximumValueRestriction");
      }
    }
  }
}