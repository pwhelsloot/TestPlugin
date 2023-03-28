using AMCS.Data;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.IntegrationTests.TestProperties;
using NodaTime;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.Steps
{
  [Binding]
  public class TermExpressionParserSteps : BaseTest
  {
    private const string DATE_TIME = "DATETIME";
    private const string LOCAL_DATE_TIME = "LOCALDATETIME";
    private const string ZONED_DATE_TIME = "ZONEDDATETIME";
    private const string ZONED_DATE = "ZONEDDATE";
    private string InputString = string.Empty;
    private int InputInteger;
    private double InputDouble;
    private decimal InputDecimal;
    private bool? InputBooleanValue;
    private DateTime? InputDateTimeValue;
    private int[] InputIntegerArray = new int[] { };
    private double[] InputDoubleArray = new double[] { };
    private decimal[] InputDecimalArray = new decimal[] { };
    private string ExpectedResult = string.Empty;
    private IExpression? ActualExpression;
    private IExpression? AdditionalActualExpression;
    private string? DateOne;
    private string? DateTwo;
    private ZonedDateTime ZonedDateTimeOne;
    private ZonedDateTime ZonedDateTimeTwo;
    string field = "EnumProp";
    object? Value;

    [Given(@"various (.*) strings (.*) expected result")]
    public void GivenStringsandExpectedResult(string inputString, string expectedResult)
    {
      InputString = inputString;
      ExpectedResult = expectedResult;
    }

    [Given(@"various (.*) integer (.*) expected result")]
    public void GivenVariousIntegerIntPropEqExpectedResult(string input, string expectedResult)
    {
      if (input.Contains(","))
      {
        var stringArray = input.Split(',').ToArray();
        InputIntegerArray = new int[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
          InputIntegerArray[i] = int.Parse(stringArray[i]);
      }
      if (!string.IsNullOrEmpty(input) && !input.Contains(","))
        InputInteger = int.Parse(input);
      ExpectedResult = expectedResult;
    }

    [Given(@"various (.*) boolean (.*) expected result")]
    public void GivenVariousTrueBooleanBoolPropEqTrueExpectedResult(string inputBooleanValue, string expectedResult)
    {
      if (!string.IsNullOrEmpty(inputBooleanValue))
      {
        if (inputBooleanValue.ToUpperInvariant().Equals("TRUE"))
        {
          InputBooleanValue = true;
        }
        else if (inputBooleanValue.ToUpperInvariant().Equals("FALSE"))
        {
          InputBooleanValue = false;
        }
      }
      else
      {
        InputBooleanValue = null;
      }
      ExpectedResult = expectedResult;
    }

    [Given(@"various (.*) double (.*) expected result")]
    public void GivenVariousDoubleDoublePropEqExpectedResult(string inputValue, string expectedResult)
    {
      if (inputValue.Contains(','))
      {
        var stringArray = inputValue.Split(',').ToArray();
        InputDoubleArray = new double[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
          InputDoubleArray[i] = double.Parse(stringArray[i]);
      }
      if (!string.IsNullOrEmpty(inputValue) && !inputValue.Contains(','))
        InputDouble = double.Parse(inputValue);
      ExpectedResult = expectedResult;
    }

    [Given(@"various (.*) decimal (.*) expected result")]
    public void GivenVariousDecimalInputAndExpectedResult(string inputValue, string expectedResult)
    {
      if (inputValue.Contains(','))
      {
        var stringArray = inputValue.Split(',').ToArray();
        InputDecimalArray = new decimal[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
        {
          stringArray[i] = stringArray[i].Remove(stringArray[i].Length - 1, 1);
          InputDecimalArray[i] = decimal.Parse(stringArray[i]);
        }
      }
      if (!string.IsNullOrEmpty(inputValue) && !inputValue.Contains(','))
      {
        inputValue = inputValue.Remove(inputValue.Length - 1, 1);
        InputDecimal = decimal.Parse(inputValue);
      }
      ExpectedResult = expectedResult;
    }

    [Given(@"various (.*) dateTime (.*) expected result")]
    public void GivenVariousDateTimeAndExpectedResult(DateTime? inputValue, string expectedResult)
    {
      InputDateTimeValue = inputValue;
      ExpectedResult = expectedResult;
    }

    [Given(@"two dates dateOne (.*) dateTwo (.*) expected result (.*)")]
    public void GivenTwoDatesDateOneDateTwo(string dateOne, string dateTwo, string expectedResult)
    {
      DateOne = dateOne;
      DateTwo = dateTwo;
      ExpectedResult = expectedResult;
    }

    [Given(@"various (.*) enum (.*) expected result")]
    public void GivenEnumAndExpectedResult(string enumValue, string expectedResult)
    {
      if (!enumValue.Contains(','))
      {
        switch (enumValue.ToUpperInvariant())
        {
          case "ABC":
            Value = FilterEntity.EnumPropEnum.abc;
            break;
          case "DEF":
            Value = FilterEntity.EnumPropEnum.def;
            break;
          case "0":
            Value = (int)FilterEntity.EnumPropEnum.abc;
            break;
        }
      }
      if (enumValue.Contains(','))
      {
        var enumArray = enumValue.Split(',').ToArray();
        foreach (string enumString in enumArray)
        {
          if (enumString.ToUpperInvariant().Equals("ABC"))
            Value = FilterEntity.EnumPropEnum.abc;
          else if (enumString.ToUpperInvariant().Equals("DEF"))
            Value = FilterEntity.EnumPropEnum.def;
          else if (enumString.ToUpperInvariant().Equals("0"))
            Value = (int)FilterEntity.EnumPropEnum.abc;
        }
      }
      ExpectedResult = expectedResult;
    }

    [When(@"expression built with input enum")]
    public void WhenExpressionBuiltWithInputEnum()
    {
      var result = ExpectedResult.ToUpperInvariant();
      if (result.Contains(" EQ"))
      {
        if (Value != null)
          ActualExpression = Expression.Eq(field, Value);
      }
      else if (result.Contains(" IN"))
      {
        var InputEnumArray = new Enum[2] { FilterEntity.EnumPropEnum.abc, FilterEntity.EnumPropEnum.def };
        ActualExpression = Expression.In(field, InputEnumArray);
      }
    }

    [When(@"these dates are converted to a particular (.*)")]
    public void WhenTheseDatesAreConvertedToAParticularZoneDates(string zoneDateTimeType)
    {
      switch (zoneDateTimeType.ToUpperInvariant())
      {
        case ZONED_DATE:
          var pattern = NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd", TimeZoneUtils.DateTimeZoneProvider);
          ZonedDateTimeOne = pattern.Parse(DateOne).Value;
          ZonedDateTimeTwo = pattern.Parse(DateTwo).Value;
          break;
        case ZONED_DATE_TIME:
          NodaTime.OffsetDateTime offsetDatetime = TimeZoneUtils.OffsetDateTimePattern.Parse(DateOne).Value;
          ZonedDateTimeOne = offsetDatetime.InZone(TimeZoneUtils.NeutralTimeZone);
          NodaTime.OffsetDateTime offsetDatetime1 = TimeZoneUtils.OffsetDateTimePattern.Parse(DateTwo).Value;
          ZonedDateTimeTwo = offsetDatetime1.InZone(TimeZoneUtils.NeutralTimeZone);
          break;
      }
    }

    [When(@"expression built with input (.*) dateTime")]
    public void WhenExpressionBuiltWithInputDateTime(string dateTimeType)
    {
      switch (dateTimeType.ToUpperInvariant())
      {
        case DATE_TIME:
          field = "DateProp";
          Value = InputDateTimeValue;
          break;
        case LOCAL_DATE_TIME:
          field = "LocalDateProp";
          Value = InputDateTimeValue;
          break;
        case ZONED_DATE:
        case ZONED_DATE_TIME:
          field = "ZonedDateTimeProp";
          Value = ZonedDateTimeOne;
          break;
      }
      if (InputDateTimeValue != null || ZonedDateTimeOne != null || ZonedDateTimeTwo != null)
      {
        var result = ExpectedResult.ToUpperInvariant();
        if (result.Contains(" EQ"))
        {
          if (Value != null)
            ActualExpression = Expression.Eq(field, Value);
        }
        else if (result.Contains(" NE"))
        {
          ActualExpression = Expression.Ne(field, Value);
        }
        else if (result.Contains(" GT") && !result.Contains(" GTE"))
        {
          ActualExpression = Expression.Gt(field, Value);
        }
        else if (result.Contains(" GTE"))
        {
          ActualExpression = Expression.Ge(field, Value);
        }
        else if (result.Contains(" LT") && !result.Contains(" LTE"))
        {
          ActualExpression = Expression.Lt(field, Value);
        }
        else if (result.Contains(" LTE"))
        {
          ActualExpression = Expression.Le(field, Value);
        }
        else if (result.Contains(" IN") && !(result.Contains(" FROM") && result.Contains(" TO")))
        {
          if (field != "ZonedDateTimeProp")
          {
            var InputDateTimeArray = new DateTime[3];
            for (int i = 0; i < 3; i++)
            {
              DateTime value = InputDateTimeValue.GetValueOrDefault();
              InputDateTimeArray[i] = value.AddDays(i);
            }
            ActualExpression = Expression.In(field, InputDateTimeArray);
          }
          else
          {
            var InputDateTimeArray = new ZonedDateTime[2] { ZonedDateTimeOne, ZonedDateTimeTwo };
            ActualExpression = Expression.In(field, InputDateTimeArray);
          }
        }
        else if (result.Contains(" FROM") && result.Contains(" TO"))
        {
          if (field != "ZonedDateTimeProp")
          {
            DateTime value = InputDateTimeValue.GetValueOrDefault();
            ActualExpression = Expression.Ge(field, value);
            AdditionalActualExpression = Expression.Le(field, value.AddMonths(1).AddDays(7));
          }
          else
          {
            ActualExpression = Expression.Ge(field, ZonedDateTimeOne);
            AdditionalActualExpression = Expression.Le(field, ZonedDateTimeTwo);
          }
        }
      }
    }

    [When(@"expression built with input decimal")]
    public void WhenExpressionBuiltWithInputDecimal()
    {
      if (InputDecimal != 0 || InputDecimalArray.Length != 0)
      {
        var result = ExpectedResult.ToUpperInvariant();
        if (result.Contains("EQ"))
        {
          ActualExpression = Expression.Eq("DecimalProp", InputDecimal);
        }
        else if (result.Contains("NE"))
        {
          ActualExpression = Expression.Ne("DecimalProp", InputDecimal);
        }
        else if (result.Contains("GT") && !result.Contains("GTE"))
        {
          ActualExpression = Expression.Gt("DecimalProp", InputDecimal);
        }
        else if (result.Contains("GTE"))
        {
          ActualExpression = Expression.Ge("DecimalProp", InputDecimal);
        }
        else if (result.Contains("LT") && !result.Contains("LTE"))
        {
          ActualExpression = Expression.Lt("DecimalProp", InputDecimal);
        }
        else if (result.Contains("LTE"))
        {
          ActualExpression = Expression.Le("DecimalProp", InputDecimal);
        }
        else if (result.Contains("IN") && !(result.Contains("FROM") && result.Contains("TO")))
        {
          ActualExpression = Expression.In("DecimalProp", InputDecimalArray);
        }
        else if (result.Contains("FROM") && result.Contains("TO"))
        {
          ActualExpression = Expression.Ge("DecimalProp", InputDecimalArray[0]);
          AdditionalActualExpression = Expression.Le("DecimalProp", InputDecimalArray[1]);
        }
      }
    }

    [When(@"expression built with input double")]
    public void WhenExpressionBuiltWithInputDouble()
    {
      if (InputDouble != 0 || InputDoubleArray.Length != 0)
      {
        var result = ExpectedResult.ToUpperInvariant();
        if (result.Contains("EQ"))
        {
          ActualExpression = Expression.Eq("DoubleProp", InputDouble);
        }
        else if (result.Contains("NE"))
        {
          ActualExpression = Expression.Ne("DoubleProp", InputDouble);
        }
        else if (result.Contains("GT") && !result.Contains("GTE"))
        {
          ActualExpression = Expression.Gt("DoubleProp", InputDouble);
        }
        else if (result.Contains("GTE"))
        {
          ActualExpression = Expression.Ge("DoubleProp", InputDouble);
        }
        else if (result.Contains("LT") && !result.Contains("LTE"))
        {
          ActualExpression = Expression.Lt("DoubleProp", InputDouble);
        }
        else if (result.Contains("LTE"))
        {
          ActualExpression = Expression.Le("DoubleProp", InputDouble);
        }
        else if (result.Contains("IN") && !(result.Contains("FROM") && result.Contains("TO")))
        {
          ActualExpression = Expression.In("DoubleProp", InputDoubleArray);
        }
        else if (result.Contains("FROM") && result.Contains("TO"))
        {
          ActualExpression = Expression.Ge("DoubleProp", InputDoubleArray[0]);
          AdditionalActualExpression = Expression.Le("DoubleProp", InputDoubleArray[1]);
        }
      }
    }

    [When(@"expression built with input boolean")]
    public void WhenExpressionBuiltWithInputBoolean()
    {
      if (InputBooleanValue != null)
      {
        var result = ExpectedResult.ToUpperInvariant();
        if (result.Contains("EQ"))
        {
          ActualExpression = Expression.Eq("BoolProp", InputBooleanValue);
        }
        else if (result.Contains("NE"))
        {
          ActualExpression = Expression.Ne("BoolProp", InputBooleanValue);
        }
      }
    }

    [When(@"expression built with input integer")]
    public void WhenExpressionBuiltWithInputInteger()
    {
      if (InputInteger != 0 || InputIntegerArray.Length != 0)
      {
        var result = ExpectedResult.ToUpperInvariant();
        if (result.Contains("EQ"))
        {
          ActualExpression = Expression.Eq("IntProp", InputInteger);
        }
        else if (result.Contains("NE"))
        {
          ActualExpression = Expression.Ne("IntProp", InputInteger);
        }
        else if (result.Contains("GT") && !result.Contains("GTE"))
        {
          ActualExpression = Expression.Gt("IntProp", InputInteger);
        }
        else if (result.Contains("GTE"))
        {
          ActualExpression = Expression.Ge("IntProp", InputInteger);
        }
        else if (result.Contains("LT") && !result.Contains("LTE"))
        {
          ActualExpression = Expression.Lt("IntProp", InputInteger);
        }
        else if (result.Contains("LTE"))
        {
          ActualExpression = Expression.Le("IntProp", InputInteger);
        }
        else if (result.Contains("IN") && !(result.Contains("FROM") && result.Contains("TO")))
        {
          ActualExpression = Expression.In("IntProp", InputIntegerArray);
        }
        else if (result.Contains("FROM") && result.Contains("TO"))
        {
          ActualExpression = Expression.Ge("IntProp", InputIntegerArray[0]);
          AdditionalActualExpression = Expression.Le("IntProp", InputIntegerArray[1]);
        }
      }
    }

    [When(@"expression built with input strings")]
    public void WhenExpressionIsBuilt()
    {
      if (!string.IsNullOrEmpty(InputString))
      {
        var result = ExpectedResult.ToUpperInvariant();
        if (result.Contains("EQ"))
        {
          ActualExpression = Expression.Eq("StringProp", InputString);
        }
        else if (result.Contains("NE"))
        {
          ActualExpression = Expression.Ne("StringProp", InputString);
        }
        else if (result.Contains("STARTSWITH"))
        {
          ActualExpression = Expression.Like("StringProp", Like.StartsWith(InputString));
        }
        else if (result.Contains("ENDSWITH"))
        {
          ActualExpression = Expression.Like("StringProp", Like.EndsWith(InputString));
        }
        else if (result.Contains("CONTAINS"))
        {
          ActualExpression = Expression.Like("StringProp", Like.Contains(InputString));
        }
        else if (result.Contains("IN"))
        {
          ActualExpression = Expression.In("StringProp", InputString.Split(',').ToArray());
        }
      }
    }

    [Then(@"output matches expected result")]
    public void ThenOutputMatchesExpectedResult()
    {
      if (ActualExpression != null && AdditionalActualExpression == null)
      {
        Filter.AssertFilter(
             ExpectedResult,
             p => p.Add(ActualExpression));
      }
      else if (ActualExpression != null && AdditionalActualExpression != null)
      {
        Filter.AssertFilter(
             ExpectedResult,
             p => p.Add(ActualExpression)
             .Add(AdditionalActualExpression));
      }
      else if (ExpectedResult.ToString().ToUpperInvariant().Contains("DATEPROP EQ"))
      {
        Filter.Fails<Exception>(ExpectedResult);
      }
      else
      {
        Filter.Fails(ExpectedResult);
      }
    }
  }
}