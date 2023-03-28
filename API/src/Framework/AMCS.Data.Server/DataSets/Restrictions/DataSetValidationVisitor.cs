namespace AMCS.Data.Server.DataSets.Restrictions
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using AMCS.Data.Configuration.Mapping.Translate;
  using AMCS.Data.Support;
  using Data.Util.Extension;
  using NodaTime;

  public class DataSetValidationVisitor : IDataSetRestrictionVisitor
  {
    private static readonly DataSetValidationError[] EmptyErrors = new DataSetValidationError[0];

    public static IList<DataSetValidationError> GetMessages(DataSet dataSet, IDataSetRecord record)
    {
      if (dataSet.Restrictions.Count == 0)
        return EmptyErrors;

      var visitor = new DataSetValidationVisitor(record);

      foreach (var restriction in dataSet.Restrictions)
      {
        restriction.Accept(visitor);
      }

      return visitor.Messages;
    }

    private readonly IDataSetRecord record;
    private readonly BusinessObjectStringTranslator translator;

    public IList<DataSetValidationError> Messages { get; } = new List<DataSetValidationError>();

    public DataSetValidationVisitor(IDataSetRecord record)
    {
      this.record = record;

      translator = new BusinessObjectStringTranslator(GetType().FullName, typeof(Strings));
    }

    private string GetLocalisedString(Strings value)
    {
      return translator.GetLocalisedString((int)value);
    }

    private string GetLocalisedString(Strings value, params object[] args)
    {
      return string.Format(GetLocalisedString(value), args);
    }

    private void AddMessage(DataSetRestriction restriction, Strings value)
    {
      Messages.Add(new DataSetValidationError(restriction, GetLocalisedString(value)));
    }

    private void AddMessage(DataSetRestriction restriction, Strings value, params object[] args)
    {
      Messages.Add(new DataSetValidationError(restriction, GetLocalisedString(value, args)));
    }

    private object GetValue(DataSetRestriction restriction)
    {
      return restriction.Column.Property.GetValue(record);
    }

    private ZonedDateTime? GetDateTimeValue(DataSetRestriction restriction)
    {
      object value = GetValue(restriction);

      switch (value)
      {
        case null:
          return null;
        case ZonedDateTime zonedDateTime:
          return zonedDateTime;
        case OffsetDateTime offsetDateTime:
          return offsetDateTime.InFixedZone();
        case DateTime dateTime:
          return TimeZoneUtils.NeutralTimeZone.AtLeniently(LocalDateTime.FromDateTime(dateTime));
        case DateTimeOffset dateTimeOffset:
          return OffsetDateTime.FromDateTimeOffset(dateTimeOffset).InFixedZone();
        case LocalDateTime localDateTime:
          return TimeZoneUtils.NeutralTimeZone.AtLeniently(localDateTime);
        default:
          throw new DataSetException($"Cannot convert '{value.GetType()}' to a zoned date/time");
      }
    }

    private decimal? GetDecimalValue(DataSetRestriction restriction)
    {
      object value = GetValue(restriction);
      if (value == null)
        return null;

      if (ValueCoercion.TryCoerce(value, typeof(decimal), out var result))
        return (decimal)result;

      throw new DataSetException($"Cannot convert '{value.GetType()}' to a decimal");
    }

    private bool IsDateRangeValid(DataSetRestriction restriction, ZonedDateTime? minimum, ZonedDateTime? maximum)
    {
      var value = GetDateTimeValue(restriction);
      if (!value.HasValue)
        return true;

      return
        (!minimum.HasValue || ZonedDateTime.Comparer.Instant.Compare(value.Value, minimum.Value) >= 0) &&
        (!maximum.HasValue || ZonedDateTime.Comparer.Instant.Compare(value.Value, maximum.Value) <= 0);
    }

    private bool IsNumericRangeValid(DataSetRestriction restriction, decimal? minimum, decimal? maximum)
    {
      var value = GetDecimalValue(restriction);
      if (!value.HasValue)
        return true;

      return
        (!minimum.HasValue || value.Value >= minimum.Value) &&
        (!maximum.HasValue || value.Value <= maximum.Value);
    }

    public void VisitDateMaximum(DataSetDateMaximumRestriction restriction)
    {
      if (!IsDateRangeValid(restriction, null, restriction.Value))
        AddMessage(restriction, Strings.DateCannotBePast, restriction.Value);
    }

    public void VisitDateMinimum(DataSetDateMinimumRestriction restriction)
    {
      if (!IsDateRangeValid(restriction, restriction.Value, null))
        AddMessage(restriction, Strings.DateCannotBeBefore, restriction.Value);
    }

    public void VisitDateRange(DataSetDateRangeRestriction restriction)
    {
      if (!IsDateRangeValid(restriction, restriction.Minimum, restriction.Maximum))
        AddMessage(restriction, Strings.DateMustBeBetween, restriction.Minimum, restriction.Maximum);
    }

    public void VisitDigits(DataSetDigitsRestriction restriction)
    {
      object value = GetValue(restriction);
      if (value == null)
        return;

      bool valid = true;
      decimal decimalValue = 0;

      if (value is string stringValue)
      {
        if (stringValue.Length == 0)
          return;

        valid = decimal.TryParse(stringValue, out decimalValue);
      }
      else if (ValueCoercion.TryCoerce(value, typeof(decimal), out var result))
      {
        decimalValue = (decimal)result;
      }
      else
      {
        valid = false;
      }

      if (valid)
      {
        string printed = Math.Abs(decimalValue).ToString(CultureInfo.InvariantCulture);

        int pos = printed.IndexOf(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator, StringComparison.OrdinalIgnoreCase);

        int precision = pos == -1 ? printed.Length : pos;
        int scale = pos == -1 ? 0 : printed.Length - pos - 1;

        if (precision == 1 && printed[0] == '0')
          precision = 0;

        if (restriction.Precision.HasValue && precision > restriction.Precision.Value)
          valid = false;
        if (restriction.Scale.HasValue && scale > restriction.Scale.Value)
          valid = false;
      }

      if (!valid)
      {
        if (restriction.Precision.HasValue)
        {
          if (restriction.Scale.HasValue)
            AddMessage(restriction, Strings.NumberPrecisionAndScaleTooLarge, restriction.Precision, restriction.Scale);
          else
            AddMessage(restriction, Strings.NumberPrecisionTooLarge, restriction.Precision);
        }
        else if (restriction.Scale.HasValue)
        {
          AddMessage(restriction, Strings.NumberScaleTooLarge, restriction.Scale);
        }
      }
    }

    public void VisitFuture(DataSetFutureRestriction restriction)
    {
      if (!IsDateRangeValid(restriction, SystemClock.Instance.GetCurrentInstant().InUtc(), null))
        AddMessage(restriction, Strings.DateMustBeFuture);
    }

    public void VisitLength(DataSetLengthRestriction restriction)
    {
      if (GetValue(restriction) is string stringValue)
      {
        if (restriction.Minimum.HasValue)
        {
          if (restriction.Maximum.HasValue)
          {
            if (stringValue.Length < restriction.Minimum.Value || stringValue.Length > restriction.Maximum.Value)
              AddMessage(restriction, Strings.LengthMustBeBetween, restriction.Minimum, restriction.Maximum);
          }
          else
          {
            if (stringValue.Length < restriction.Minimum.Value)
              AddMessage(restriction, Strings.LengthMustBeAtLeast, restriction.Minimum);
          }
        }
        else if (restriction.Maximum.HasValue)
        {
          if (stringValue.Length > restriction.Maximum.Value)
            AddMessage(restriction, Strings.LengthCannotBeMoreThan, restriction.Maximum);
        }
      }
    }

    public void VisitMaximum(DataSetMaximumRestriction restriction)
    {
      if (!IsNumericRangeValid(restriction, null, restriction.Value))
        AddMessage(restriction, Strings.NumberCannotBeMoreThan, restriction.Value);
    }

    public void VisitMinimum(DataSetMinimumRestriction restriction)
    {
      if (!IsNumericRangeValid(restriction, restriction.Value, null))
        AddMessage(restriction, Strings.NumberCannotBeLessThan, restriction.Value);
    }

    public void VisitNotEmpty(DataSetNotEmptyRestriction restriction)
    {
      object value = GetValue(restriction);

      bool valid = value != null;

      if (valid && value is string)
        valid = !Equals(value, "");

      if (!valid)
        AddMessage(restriction, Strings.CannotBeEmpty);
    }

    public void VisitPast(DataSetPastRestriction restriction)
    {
      if (!IsDateRangeValid(restriction, null, SystemClock.Instance.GetCurrentInstant().InUtc()))
        AddMessage(restriction, Strings.DateMustBePast);
    }

    public void VisitRange(DataSetRangeRestriction restriction)
    {
      if (!IsNumericRangeValid(restriction, restriction.Minimum, restriction.Maximum))
        AddMessage(restriction, Strings.NumberMustBeBetween, restriction.Minimum, restriction.Maximum);
    }

    public void VisitList(DataSetListRestriction restriction)
    {
      object value = GetValue(restriction);
      if (value == null)
        return;

      if (restriction.List.Find(value) == null)
        AddMessage(restriction, Strings.ValueMustBeInList);
    }

    public void VisitReference(DataSetReferenceRestriction restriction)
    {
    }

    private enum Strings
    {
      [StringValue("Cannot be empty")]
      CannotBeEmpty,

      [StringValue("Date cannot be past '{0}'")]
      DateCannotBePast,

      [StringValue("Date cannot be before '{0}'")]
      DateCannotBeBefore,

      [StringValue("Date must be between '{0}' and '{1}'")]
      DateMustBeBetween,

      [StringValue("Date must lie in the future")]
      DateMustBeFuture,

      [StringValue("Date must lie in the past")]
      DateMustBePast,

      [StringValue("Length must be between {0} and {1}")]
      LengthMustBeBetween,

      [StringValue("Length must be at least {0}")]
      LengthMustBeAtLeast,

      [StringValue("Length cannot be more than {0}")]
      LengthCannotBeMoreThan,

      [StringValue("Number cannot be more than {0}")]
      NumberCannotBeMoreThan,

      [StringValue("Number cannot be less than {0}")]
      NumberCannotBeLessThan,

      [StringValue("Number must be between {0} and {1}")]
      NumberMustBeBetween,

      [StringValue("Numeric precision cannot be more than {0}, and scale {1}")]
      NumberPrecisionAndScaleTooLarge,

      [StringValue("Numeric precision cannot be more than {0}")]
      NumberPrecisionTooLarge,

      [StringValue("Numeric scale cannot be more than {0}")]
      NumberScaleTooLarge,

      [StringValue("Value does not appear in the list")]
      ValueMustBeInList
    }
  }
}
