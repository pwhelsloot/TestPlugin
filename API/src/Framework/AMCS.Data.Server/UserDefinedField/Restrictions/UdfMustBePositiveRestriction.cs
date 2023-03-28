namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;
  using NodaTime.Text;

  public class UdfMustBePositiveRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      switch (udfField.DataType)
      {
        case DataType.Duration:
          var duration = DurationPattern.Roundtrip.Parse(resultObject.StringValue);

          if (duration.Value.BclCompatibleTicks < 0)
            throw new UdfValidationException($"Duration must be a positive value on {udfField.Namespace}.{udfField.FieldName}");
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfMustBePositiveRestriction");
      }
    }
  }
}