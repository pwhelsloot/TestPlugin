namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using System;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;
  using NodaTime;
  using NodaTime.Extensions;

  public class UdfMustBeFutureRestriction : IUdfRestriction
  {
    public void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata)
    {
      var clock = SystemClock.Instance;

      switch (udfField.DataType)
      {
        case DataType.OffsetDateTime:
          var parsedOffsetDateTime = TimeZoneUtils.OffsetDateTimePattern.Parse(resultObject.StringValue).Value;
          var offsetDateTime = clock.InUtc().GetCurrentOffsetDateTime().WithOffset(parsedOffsetDateTime.Offset);

          if (OffsetDateTime.Comparer.Instant.Compare(parsedOffsetDateTime, offsetDateTime) == -1)
            throw new UdfValidationException($"OffsetDateTime must be a future value on {udfField.Namespace}.{udfField.FieldName}");
          break;

        case DataType.ZonedDateTime:
        case DataType.UtcDateTime:
          var parsedZoneDateTime = TimeZoneUtils.ZonedDateTimePattern.Parse(resultObject.StringValue).Value;
          var zonedDateTime = clock.InZone(parsedZoneDateTime.Zone).GetCurrentZonedDateTime();

          if (ZonedDateTime.Comparer.Instant.Compare(parsedZoneDateTime, zonedDateTime) == -1)
          {
            throw new UdfValidationException(
              $"{(udfField.DataType == DataType.ZonedDateTime ? DataType.ZonedDateTime : DataType.UtcDateTime)} must be a future value on {udfField.Namespace}.{udfField.FieldName}");
          }
          break;

        default:
          throw new NotSupportedException($"Invalid DataType of {udfField.DataType} was passed to UdfMustBeFutureRestriction");
      }
    }
  }
}