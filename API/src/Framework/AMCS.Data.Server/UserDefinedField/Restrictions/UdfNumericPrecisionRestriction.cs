namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using System.Runtime.InteropServices;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  public class UdfNumericPrecisionRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      switch (udfField.DataType)
      {
        case DataType.Decimal:
          var currentPrecision = Convert.ToInt32(UdfJsonUtil.GetSingleUdfRestrictionValue(metadata));

          if (!resultObject.DecimalValue.HasValue || GetScale(resultObject.DecimalValue.Value) != currentPrecision)
            throw new UdfValidationException($"Numeric precision must be {currentPrecision} for {udfField.Namespace}.{udfField.FieldName}");
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfNumericPrecisionRestriction");
      }
    }

    private static int GetScale(decimal value)
    {
      return new DecimalScale(value).Scale;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct DecimalScale
    {
      public DecimalScale(decimal value)
      {
        this = default;
        this.decimalValue = value;
      }

      [FieldOffset(0)] 
      private decimal decimalValue;

      [FieldOffset(0)]
      private int flags;

      public int Scale => (flags >> 16) & 0xff;
    }
  }
}