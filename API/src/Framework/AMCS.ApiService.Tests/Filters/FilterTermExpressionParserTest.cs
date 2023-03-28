using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Configuration.TimeZones.Moment;
using AMCS.Data.Mocking;
using AMCS.Data.Server.SQL.Querying;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.ApiService.Tests.Filters
{
    [TestClass]
    public class FilterTermExpressionParserTest : BaseTest
    {
        [TestMethod]
        public void ParseStringTerm()
        {
            Filter.AssertFilter(
              "stringProp eQ 'abc123'",
              p => p.Add(Expression.Eq("StringProp", "abc123")));

            Filter.AssertFilter(
              "\t    stringProp    \t NE    \t    'def456'  \t  ",
              p => p.Add(Expression.Ne("StringProp", "def456")));

            Filter.AssertFilter(
              "stringProp startsWITh 'abc'",
              p => p.Add(Expression.Like("StringProp", Like.StartsWith("abc"))));

            Filter.AssertFilter(
              "stringProp conTains 'c12'",
              p => p.Add(Expression.Like("StringProp", Like.Contains("c12"))));

            Filter.AssertFilter(
              "stringProp endswith '123'",
              p => p.Add(Expression.Like("StringProp", Like.EndsWith("123"))));

            Filter.AssertFilter(
              "stringProp IN ('hello', 'there')",
              p => p.Add(Expression.In("StringProp", new[] { "hello", "there" })));

            Filter.Fails("stringProp lt 5");

            Filter.Fails("stringProp lte 5");

            Filter.Fails("stringProp gt 5");

            Filter.Fails("stringProp gte 5");

            Filter.Fails("invalidProp eq 'abc123'");

            Filter.Fails("stringProp between 'abc' and 'def'");
        }

        public void ParseBooleanTerm()
        {
            Filter.AssertFilter(
              "boolProp eq true",
              p => p.Add(Expression.Eq("BoolProp", true)));

            Filter.AssertFilter(
              "boolProp eq false",
              p => p.Add(Expression.Eq("BoolProp", false)));

            Filter.AssertFilter(
              "boolProp ne true",
              p => p.Add(Expression.Ne("BoolProp", true)));

            Filter.AssertFilter(
              "boolProp ne false",
              p => p.Add(Expression.Ne("BoolProp", false)));

            Filter.Fails("boolProp lt true");

            Filter.Fails("boolProp lte true");

            Filter.Fails("boolProp gt true");

            Filter.Fails("boolProp gte true");

            Filter.Fails("boolProp startswith true");

            Filter.Fails("boolProp contains false");

            Filter.Fails("boolProp endswith true");

            Filter.Fails("boolProp eq 'abc'");

            Filter.Fails("boolProp in (true,false)");

            Filter.Fails("boolProp between true and false");
        }

        [TestMethod]
        public void ParseIntTerm()
        {
            Filter.AssertFilter(
              "intProp eq 851",
              p => p.Add(Expression.Eq("IntProp", 851)));

            Filter.AssertFilter(
              "intProp ne 853",
              p => p.Add(Expression.Ne("IntProp", 853)));

            Filter.AssertFilter(
              "intProp lt 853",
              p => p.Add(Expression.Lt("IntProp", 853)));

            Filter.AssertFilter(
              "intProp lte 853",
              p => p.Add(Expression.Le("IntProp", 853)));

            Filter.AssertFilter(
              "intProp gt 853",
              p => p.Add(Expression.Gt("IntProp", 853)));

            Filter.AssertFilter(
              "intProp gte 853",
              p => p.Add(Expression.Ge("IntProp", 853)));

            Filter.AssertFilter(
              "intProp in (1,2,3, 4,   \t5)",
              p => p.Add(Expression.In("IntProp", new[] { 1, 2, 3, 4, 5 })));

            Filter.AssertFilter(
              "intProp from 10 to   3000",
              p => p
                .Add(Expression.Ge("IntProp", 10))
                .Add(Expression.Le("IntProp", 3000)));

            Filter.Fails("intProp startswith 1");

            Filter.Fails("intProp contains 1");

            Filter.Fails("intProp endswith 1");

            Filter.Fails("intProp eq 'abc'");
        }

        [TestMethod]
        public void ParseDoubleTerm()
        {
            Filter.AssertFilter(
              "doubleProp eq 12.34",
              p => p.Add(Expression.Eq("DoubleProp", 12.34)));

            Filter.AssertFilter(
              "doubleProp ne 34.567",
              p => p.Add(Expression.Ne("DoubleProp", 34.567)));

            Filter.AssertFilter(
              "doubleProp lt 34.567",
              p => p.Add(Expression.Lt("DoubleProp", 34.567)));

            Filter.AssertFilter(
              "doubleProp lte 34.567",
              p => p.Add(Expression.Le("DoubleProp", 34.567)));

            Filter.AssertFilter(
              "doubleProp gt 34.567",
              p => p.Add(Expression.Gt("DoubleProp", 34.567)));

            Filter.AssertFilter(
              "doubleProp gte 34.567",
              p => p.Add(Expression.Ge("DoubleProp", 34.567)));

            Filter.AssertFilter(
              "doubleProp in (1.1, 2.2, 3, 4.4,   \t5.555)",
              p => p.Add(Expression.In("DoubleProp", new[] { 1.1, 2.2, 3, 4.4, 5.555 })));

            Filter.AssertFilter(
              "doubleProp FrOm 10.125 tO  3000.985",
              p => p
                .Add(Expression.Ge("DoubleProp", 10.125))
                .Add(Expression.Le("DoubleProp", 3000.985)));

            Filter.Fails("doubleProp startswith 1");

            Filter.Fails("doubleProp contains 1");

            Filter.Fails("doubleProp endswith 1");

            Filter.Fails("doubleProp eq 'abc'");
        }

        [TestMethod]
        public void ParseDecimalTerm()
        {
            Filter.AssertFilter(
              "decimalProp eq 12.34",
              p => p.Add(Expression.Eq("DecimalProp", 12.34m)));

            Filter.AssertFilter(
              "decimalProp ne 34.567",
              p => p.Add(Expression.Ne("DecimalProp", 34.567m)));

            Filter.AssertFilter(
              "decimalProp lt 34.567",
              p => p.Add(Expression.Lt("DecimalProp", 34.567m)));

            Filter.AssertFilter(
              "decimalProp lte 34.567",
              p => p.Add(Expression.Le("DecimalProp", 34.567m)));

            Filter.AssertFilter(
              "decimalProp gt 34.567",
              p => p.Add(Expression.Gt("DecimalProp", 34.567m)));

            Filter.AssertFilter(
              "decimalProp gte 34.567",
              p => p.Add(Expression.Ge("DecimalProp", 34.567m)));

            Filter.AssertFilter(
              "decimalProp in (1.1, 2.2, 3, 4.4,   \t5.555)",
              p => p.Add(Expression.In("DecimalProp", new[] { 1.1m, 2.2m, 3m, 4.4m, 5.555m })));

            Filter.AssertFilter(
              "decimalProp FrOm 10.125 tO  3000.985",
              p => p
                .Add(Expression.Ge("DecimalProp", 10.125m))
                .Add(Expression.Le("DecimalProp", 3000.985m)));

            Filter.Fails("decimalProp startswith 1");

            Filter.Fails("decimalProp contains 1");

            Filter.Fails("decimalProp endswith 1");

            Filter.Fails("decimalProp eq 'abc'");
        }

        [TestMethod]
        public void ParseDateTerm()
        {
            Filter.AssertFilter(
              "dateProp eq '2017-12-28'",
              p => p.Add(Expression.Eq("DateProp", new DateTime(2017, 12, 28))));

            Filter.AssertFilter(
              "dateProp ne '2017-12-22'",
              p => p.Add(Expression.Ne("DateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "dateProp lt '2017-12-22'",
              p => p.Add(Expression.Lt("DateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "dateProp lte '2017-12-22'",
              p => p.Add(Expression.Le("DateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "dateProp gt '2017-12-22'",
              p => p.Add(Expression.Gt("DateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "dateProp gte '2017-12-22'",
              p => p.Add(Expression.Ge("DateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "dateProp in ('2017-12-21','2017-12-22',   '2017-12-23') ",
              p => p.Add(Expression.In("DateProp", new[] { new DateTime(2017, 12, 21), new DateTime(2017, 12, 22), new DateTime(2017, 12, 23) })));

            Filter.AssertFilter(
              "dateProp from '2017-11-15' to '2017-12-22'",
              p => p
                .Add(Expression.Ge("DateProp", new DateTime(2017, 11, 15)))
                .Add(Expression.Le("DateProp", new DateTime(2017, 12, 22))));

            Filter.Fails("dateProp startswith '2017-12-22'");

            Filter.Fails("dateProp contains '2017-12-22'");

            Filter.Fails("dateProp endswith '2017-12-22'");

            Filter.Fails<Exception>("dateProp eq 'abc'");
        }

        [TestMethod]
        public void ParseLocalDateTerm()
        {
            Filter.AssertFilter(
              "localDateProp eq '2017-12-28'",
              p => p.Add(Expression.Eq("LocalDateProp", new DateTime(2017, 12, 28))));

            Filter.AssertFilter(
             "localDateProp ne '2017-12-22'",
             p => p.Add(Expression.Ne("LocalDateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "localDateProp lt '2017-12-22'",
              p => p.Add(Expression.Lt("LocalDateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "localDateProp lte '2017-12-22'",
              p => p.Add(Expression.Le("LocalDateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "localDateProp gt '2017-12-22'",
              p => p.Add(Expression.Gt("LocalDateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "localDateProp gte '2017-12-22'",
              p => p.Add(Expression.Ge("LocalDateProp", new DateTime(2017, 12, 22))));

            Filter.AssertFilter(
              "localDateProp in ('2017-12-21','2017-12-22',   '2017-12-23') ",
              p => p.Add(Expression.In("LocalDateProp", new[] { new DateTime(2017, 12, 21), new DateTime(2017, 12, 22), new DateTime(2017, 12, 23) })));

            Filter.AssertFilter(
              "localDateProp from '2017-11-15' to '2017-12-22'",
              p => p
                .Add(Expression.Ge("LocalDateProp", new DateTime(2017, 11, 15)))
                .Add(Expression.Le("LocalDateProp", new DateTime(2017, 12, 22))));
        }

        [TestMethod]
        public void ParseZonedDateTerm()
        {
            string dateString = "2017-12-08";
            string dateString1 = "2017-12-28";
            var pattern = NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd", TimeZoneUtils.DateTimeZoneProvider);
            ZonedDateTime zonedDateTime = pattern.Parse(dateString).Value;
            ZonedDateTime zonedDateTime1 = pattern.Parse(dateString1).Value;

            Filter.AssertFilter(
              "ZonedDateTimeProp eq '2017-12-08'",
              p => p.Add(Expression.Eq("ZonedDateTimeProp", zonedDateTime)));

            Filter.AssertFilter(
             "ZonedDateTimeProp ne '2017-12-08'",
             p => p.Add(Expression.Ne("ZonedDateTimeProp", zonedDateTime)));

            Filter.AssertFilter(
             "ZonedDateTimeProp lt '2017-12-08'",
             p => p.Add(Expression.Lt("ZonedDateTimeProp", zonedDateTime)));

            Filter.AssertFilter(
              "ZonedDateTimeProp lte '2017-12-08'",
              p => p.Add(Expression.Le("ZonedDateTimeProp", zonedDateTime)));

            Filter.AssertFilter(
              "ZonedDateTimeProp gt '2017-12-08'",
              p => p.Add(Expression.Gt("ZonedDateTimeProp", zonedDateTime)));

            Filter.AssertFilter(
              "ZonedDateTimeProp gte '2017-12-08'",
              p => p.Add(Expression.Ge("ZonedDateTimeProp", zonedDateTime)));

            Filter.AssertFilter(
              "ZonedDateTimeProp in ('2017-12-08','2017-12-28') ",
              p => p.Add(Expression.In("ZonedDateTimeProp", new[] { zonedDateTime, zonedDateTime1 })));

            Filter.AssertFilter(
              "ZonedDateTimeProp from '2017-12-08' to '2017-12-28'",
              p => p
                .Add(Expression.Ge("ZonedDateTimeProp", zonedDateTime))
                .Add(Expression.Le("ZonedDateTimeProp", zonedDateTime1)));
        }

        [TestMethod]
        public void ParseZonedDateTimeTerm()
        {
            string dateTimeString = "2017-12-08T18:43:09.477+01:00";
            string dateTimeString1 = "2017-12-28T18:43:09.477+01:00";
            NodaTime.OffsetDateTime offsetDatetime = TimeZoneUtils.OffsetDateTimePattern.Parse(dateTimeString).Value;
            ZonedDateTime zonedDateTime2 = offsetDatetime.InZone(TimeZoneUtils.NeutralTimeZone);
            NodaTime.OffsetDateTime offsetDatetime1 = TimeZoneUtils.OffsetDateTimePattern.Parse(dateTimeString1).Value;
            ZonedDateTime zonedDateTime3 = offsetDatetime1.InZone(TimeZoneUtils.NeutralTimeZone);

            Filter.AssertFilter(
              "ZonedDateTimeProp eq '2017-12-08T18:43:09.477+01:00'",
              p => p.Add(Expression.Eq("ZonedDateTimeProp", zonedDateTime2)));

            Filter.AssertFilter(
             "ZonedDateTimeProp ne '2017-12-08T18:43:09.477+01:00'",
             p => p.Add(Expression.Ne("ZonedDateTimeProp", zonedDateTime2)));

            Filter.AssertFilter(
             "ZonedDateTimeProp lt '2017-12-08T18:43:09.477+01:00'",
             p => p.Add(Expression.Lt("ZonedDateTimeProp", zonedDateTime2)));

            Filter.AssertFilter(
              "ZonedDateTimeProp lte '2017-12-08T18:43:09.477+01:00'",
              p => p.Add(Expression.Le("ZonedDateTimeProp", zonedDateTime2)));

            Filter.AssertFilter(
              "ZonedDateTimeProp gt '2017-12-08T18:43:09.477+01:00'",
              p => p.Add(Expression.Gt("ZonedDateTimeProp", zonedDateTime2)));

            Filter.AssertFilter(
              "ZonedDateTimeProp gte '2017-12-08T18:43:09.477+01:00'",
              p => p.Add(Expression.Ge("ZonedDateTimeProp", zonedDateTime2)));

            Filter.AssertFilter(
              "ZonedDateTimeProp in ('2017-12-08T18:43:09.477+01:00','2017-12-28T18:43:09.477+01:00') ",
              p => p.Add(Expression.In("ZonedDateTimeProp", new[] { zonedDateTime2, zonedDateTime3 })));

            Filter.AssertFilter(
              "ZonedDateTimeProp from '2017-12-08T18:43:09.477+01:00' to '2017-12-28T18:43:09.477+01:00'",
              p => p
                .Add(Expression.Ge("ZonedDateTimeProp", zonedDateTime2))
                .Add(Expression.Le("ZonedDateTimeProp", zonedDateTime3)));
        }
    }
}
