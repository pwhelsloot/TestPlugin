using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NodaTime;

namespace AMCS.Data.Support
{
    public static class JsonUtil
    {
        private static readonly string[] DateTimeFormats =
        {
          "yyyy-MM-ddTHH:mm:ss.fffZ",
          "yyyy-MM-dd HH:mm:ss.fff",
          "yyyy-MM-dd HH:mm:ss",
          "yyyy-MM-dd HH:mm",
          "yyyy-MM-dd",
        };

        public static DateTime ParseDateTime(string value)
        {
            var dateStyle = DateTimeStyles.None;
            // If a UTC datetime is supplied, don't parse date in server timezone, parse for UTC.
            if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, dateStyle, out var utcParseAttempt))
            {
                dateStyle = DateTimeStyles.AdjustToUniversal;
            }
            if (!DateTime.TryParseExact(value, DateTimeFormats, CultureInfo.InvariantCulture, dateStyle, out var result))
                throw new Exception("Value is not a valid date/time");
            return result;
        }

        public static string PrintDateTime(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public static DateTimeOffset ParseDateTimeOffset(string value)
        {
            if (!DateTimeOffset.TryParse(value, out var result))
                throw new Exception("Value is not a valid date/time offset");
            return result;
        }

        public static string PrintDateTimeOffset(DateTimeOffset value)
        {
            return value.ToString("o", CultureInfo.InvariantCulture);
        }

        public static void ReadStartArray(JsonReader reader)
        {
            ReadTokenType(reader, JsonToken.StartArray);
        }

        public static void ReadEndArray(JsonReader reader)
        {
            ReadTokenType(reader, JsonToken.EndArray);
        }

        public static void ReadStartObject(JsonReader reader)
        {
            ReadTokenType(reader, JsonToken.StartObject);
        }

        public static void ReadEndObject(JsonReader reader)
        {
            ReadTokenType(reader, JsonToken.EndObject);
        }

        private static void ReadTokenType(JsonReader reader, JsonToken tokenType)
        {
            Read(reader);
            ExpectTokenType(reader, tokenType);
        }

        public static void ExpectTokenType(JsonReader reader, JsonToken tokenType)
        {
            if (reader.TokenType != tokenType)
                throw new InvalidOperationException($"Unexpected {reader.TokenType}, expected {tokenType}");
        }

        public static void RejectTokenType(JsonReader reader, JsonToken tokenType)
        {
            if (reader.TokenType == tokenType)
                throw new InvalidOperationException($"Unexpected {tokenType}, expected {reader.TokenType}");
        }

        public static void Read(JsonReader reader)
        {
            if (!reader.Read())
                throw new InvalidOperationException("Unexpected end of stream");
        }

        public static void ReadForType(JsonReader reader, Type type)
        {
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                reader.ReadAsDateTime();
            else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
                reader.ReadAsDateTimeOffset();
            else
                reader.Read();
        }

        public static object Parse(object value, Type type)
        {
            // We are making the dependency on TimeZoneUtils explicit so that if it is not set up it will fail
            // and throw an exception instead of hiding it and rethrowing it later
            // See here for more details: https://stackoverflow.com/a/4737910/16919291
            TimeZoneUtils.EnsureInitialized();

            if (type == typeof(DateTime))
                return ParseDateTime((string)value);
            if (type == typeof(DateTimeOffset))
                return ParseDateTimeOffset((string)value);
            if (type == typeof(Guid))
                return Guid.Parse((string)value);
            if (type == typeof(byte[]) && value is string stringValue)
                return Convert.FromBase64String(stringValue);
            if (type == typeof(Instant))
                return TimeZoneUtils.InstantPattern.Parse((string)value).GetValueOrThrow();
            if (type == typeof(OffsetDateTime))
                return TimeZoneUtils.OffsetDateTimePattern.Parse((string)value).GetValueOrThrow();
            if (type == typeof(ZonedDateTime))
                return ConvertToZonedDateTime((string)value);
            if (type == typeof(LocalDate))
                return TimeZoneUtils.LocalDatePattern.Parse((string)value).GetValueOrThrow();
            if (type == typeof(LocalTime))
                return TimeZoneUtils.LocalTimePattern.Parse((string)value).GetValueOrThrow();
            if (type.IsEnum)
            {
                if (int.TryParse(value.ToString(), out _))
                    return Enum.ToObject(type, value);
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                  return Enum.Parse(type, value.ToString(), true);
                return null;
            }
            return value;
        }

        public static object Print(object value)
        {
            // We are making the dependency on TimeZoneUtils explicit so that if it is not set up it will fail
            // and throw an exception instead of hiding it and rethrowing it later
            // See here for more details: https://stackoverflow.com/a/4737910/16919291
            TimeZoneUtils.EnsureInitialized();

            switch (value)
            {
                case DateTime dateTime:
                    return PrintDateTime(dateTime);
                case DateTimeOffset dateTimeOffset:
                    return PrintDateTimeOffset(dateTimeOffset);
                case Guid guid:
                    return guid.ToString();
                case byte[] byteArray:
                    return Convert.ToBase64String(byteArray, Base64FormattingOptions.None);
                case Instant instant:
                    return TimeZoneUtils.InstantPattern.Format(instant);
                case OffsetDateTime offsetDateTime:
                    return TimeZoneUtils.OffsetDateTimePattern.Format(offsetDateTime);
                case ZonedDateTime zonedDateTime:
                    return TimeZoneUtils.ZonedDateTimePattern.Format(zonedDateTime);
                case LocalDate localDate:
                    return TimeZoneUtils.LocalDatePattern.Format(localDate);
                case LocalTime localTime:
                    return TimeZoneUtils.LocalTimePattern.Format(localTime);
                case Enum _:
                    return (int)value;
                default:
                    return value;
            }
        }

        public static void PrintZonedDateTime(JsonWriter json, ZonedDateTime zonedDateTime)
        {
            // We are making the dependency on TimeZoneUtils explicit so that if it is not set up it will fail
            // and throw an exception instead of hiding it and rethrowing it later
            // See here for more details: https://stackoverflow.com/a/4737910/16919291
            TimeZoneUtils.EnsureInitialized();
            json.WriteStartObject();
            json.WritePropertyName("DateTime");
            json.WriteValue(TimeZoneUtils.OffsetDateTimePattern.Format(zonedDateTime.ToOffsetDateTime()));
            json.WritePropertyName("TimeZone");
            json.WriteValue(zonedDateTime.Zone.Id);
            json.WriteEndObject();
        }

        public static string GetJsonSchemaType(Type type)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            var jsonSchema = new JsonSchemaGenerator().Generate(type);
            #pragma warning restore CS0618 // Type or member is obsolete
            JObject jsonObject = JObject.Parse(jsonSchema.ToString());
            return (string)jsonObject["type"];
        }

        private static ZonedDateTime ConvertToZonedDateTime(string value)
        {
            ZonedDateTime zonedDateTime;
            if (value.Length == 10)
            {
                DateTime dateformat;
                if (DateTime.TryParse(value, out dateformat))
                {
                    string convertedValue = dateformat.ToString("yyyy-MM-dd");
                    var pattern = NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd", TimeZoneUtils.DateTimeZoneProvider);
                    zonedDateTime = pattern.Parse(convertedValue).GetValueOrThrow();
                }
                else
                {
                    throw new InvalidOperationException("Invalid Date Format");
                }
            }
            else
            {
                value = value.Replace(" ", "+");
                NodaTime.OffsetDateTime offsetDatetime = TimeZoneUtils.OffsetDateTimePattern.Parse((string)value).GetValueOrThrow();
                zonedDateTime = offsetDatetime.InZone(TimeZoneUtils.NeutralTimeZone);
            }
            return zonedDateTime;
        }
    }
}