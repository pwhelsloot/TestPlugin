using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.Data.Configuration.TimeZones.Moment
{
  public class DatabaseConverter
  {
    private static readonly Instant MinInterval = Instant.FromUtc(1901, 1, 1, 12, 0, 0);
    private static readonly Instant MaxInterval = Instant.FromUtc(2038, 3, 14, 0, 0, 0);

    public static Stream GetPopulation()
    {
      return typeof(DatabaseConverter).Assembly.GetManifestResourceStream(typeof(DatabaseConverter).Namespace + ".population.json");
    }

    public static string ToMoment(TzdbDateTimeZoneSource source, Stream population)
    {
      return ToMoment(source, population, DatabaseConverterOptions.None);
    }

    public static string ToMoment(TzdbDateTimeZoneSource source, Stream population, DatabaseConverterOptions options)
    {
      return new DatabaseConverter(source, LoadPopulation(population), options).ToMoment();
    }

    private static Dictionary<string, long> LoadPopulation(Stream stream)
    {
      var result = new Dictionary<string, long>();
      if (stream == null)
        return result;

      JObject obj;

      using (var reader = new StreamReader(stream))
      using (var json = new JsonTextReader(reader))
      {
        obj = JObject.Load(json);
      }

      foreach (var entry in obj)
      {
        result.Add(entry.Key, (long)entry.Value);
      }

      return result;
    }

    private readonly TzdbDateTimeZoneSource source;
    private readonly Dictionary<string, long> population;
    private readonly DatabaseConverterOptions options;
    private readonly Dictionary<string, List<string>> aliasesByZoneId;
    private readonly Dictionary<string, TzdbZoneLocation> locationsByZoneId;

    private DatabaseConverter(TzdbDateTimeZoneSource source, Dictionary<string, long> population, DatabaseConverterOptions options)
    {
      this.source = source;
      this.population = population;
      this.options = options;

      aliasesByZoneId = source.Aliases.ToDictionary(p => p.Key, p => p.ToList());
      locationsByZoneId = source.ZoneLocations.ToDictionary(p => p.ZoneId, p => p);
    }

    private string ToMoment()
    {
      using (var writer = new StringWriter())
      {
        using (var json = new JsonTextWriter(writer))
        {
          Convert(json);
        }

        return writer.ToString();
      }
    }

    private void Convert(JsonTextWriter json)
    {
      json.WriteStartObject();

      json.WritePropertyName("version");
      json.WriteValue(source.TzdbVersion);

      json.WritePropertyName("zones");
      ConvertZones(json);

      json.WritePropertyName("links");
      json.WriteStartArray();
      json.WriteEndArray();

      if (options.HasFlag(DatabaseConverterOptions.IncludeCountries))
      {
        json.WritePropertyName("countries");
        WriteCountries(json);
      }

      json.WriteEndObject();
    }

    private void ConvertZones(JsonTextWriter json)
    {
      json.WriteStartArray();

      foreach (var zoneId in source.CanonicalIdMap.Keys.OrderBy(p => p))
      {
        ConvertZone(zoneId, json);
      }

      json.WriteEndArray();
    }

    private void ConvertZone(string zoneId, JsonTextWriter json)
    {
      json.WriteStartObject();

      json.WritePropertyName("name");
      json.WriteValue(zoneId);

      var zone = source.ForId(zoneId);
      var intervals = GetZoneIntervals(zone);

      json.WritePropertyName("abbrs");
      json.WriteStartArray();
      foreach (var interval in intervals)
      {
        json.WriteValue(interval.Abbr);
      }
      json.WriteEndArray();

      json.WritePropertyName("untils");
      json.WriteStartArray();
      foreach (var interval in intervals)
      {
        if (interval.Until.HasValue)
          json.WriteValue(interval.Until.Value);
        else
          json.WriteNull();
      }
      json.WriteEndArray();

      json.WritePropertyName("offsets");
      json.WriteStartArray();
      foreach (var interval in intervals)
      {
        if ((int)interval.Offset == interval.Offset)
          json.WriteValue((int)interval.Offset);
        else
          json.WriteValue(interval.Offset);
      }
      json.WriteEndArray();

      json.WritePropertyName("population");
      population.TryGetValue(zoneId, out long zonePopulation);
      json.WriteValue(zonePopulation);

      if (options.HasFlag(DatabaseConverterOptions.IncludeCountries))
      {
        json.WritePropertyName("countries");
        json.WriteStartArray();

        string countryCode;

        if (zoneId == source.CanonicalIdMap[zoneId])
        {
          if (TryGetCountryCode(source, zoneId, out countryCode))
            json.WriteValue(countryCode);
        }

        var countryCodes = new List<string>();

        if (aliasesByZoneId.TryGetValue(zoneId, out var aliases))
        {
          foreach (string alias in aliases)
          {
            if (TryGetCountryCode(source, alias, out countryCode))
              countryCodes.Add(countryCode);
          }
        }

        countryCodes.Sort(StringComparer.Ordinal);

        foreach (string item in countryCodes)
        {
          json.WriteValue(item);
        }

        json.WriteEndArray();
      }

      json.WriteEndObject();
    }

    private List<Interval> GetZoneIntervals(DateTimeZone zone)
    {
      var result = new List<Interval>();

      foreach (var zoneInterval in zone.GetZoneIntervals(new NodaTime.Interval(null, null)))
      {
        if (zoneInterval.HasEnd)
        {
          if (zoneInterval.End < MinInterval)
            continue;
          if (zoneInterval.End > MaxInterval)
            break;
        }

        var interval = GetInterval(zoneInterval);

        if (result.Count > 0)
        {
          var last = result[result.Count - 1];
          if (interval.Abbr == last.Abbr && interval.Offset == last.Offset)
            result.RemoveAt(result.Count - 1);
        }

        result.Add(interval);
      }

      if (result.Count > 0 && result[result.Count - 1].Until.HasValue)
      {
        var interval = GetInterval(zone.GetZoneInterval(Instant.MaxValue));
        if (!interval.Until.HasValue)
          result.Add(interval);
      }

      return result;
    }

    private Interval GetInterval(ZoneInterval zoneInterval)
    {
      long? until = null;
      if (zoneInterval.HasEnd)
        until = zoneInterval.End.ToUnixTimeMilliseconds();

      double offset = -(zoneInterval.WallOffset.Seconds / 60.0);
      offset = Math.Round(offset * 10000) / 10000.0;

      return new Interval(until, zoneInterval.Name, offset);
    }

    private bool TryGetCountryCode(TzdbDateTimeZoneSource source, string zoneId, out string countryCode)
    {
      if (!locationsByZoneId.TryGetValue(zoneId, out var location))
      {
        var zone = source.ForId(zoneId);
        if (zone.Id != zoneId)
          locationsByZoneId.TryGetValue(zone.Id, out location);
      }

      countryCode = location?.CountryCode;

      return countryCode != null;
    }

    private void WriteCountries(JsonTextWriter json)
    {
      var countries = new Dictionary<string, List<string>>();

      foreach (var location in source.ZoneLocations)
      {
        if (!countries.TryGetValue(location.CountryCode, out var locations))
        {
          locations = new List<string>();
          countries.Add(location.CountryCode, locations);
        }

        locations.Add(source.CanonicalIdMap[location.ZoneId]);
      }

      json.WriteStartArray();

      foreach (var country in countries.OrderBy(p => p.Key))
      {
        json.WriteStartObject();

        json.WritePropertyName("name");
        json.WriteValue(country.Key);

        json.WritePropertyName("zones");
        json.WriteStartArray();
        foreach (string zone in country.Value.OrderBy(p => p))
        {
          json.WriteValue(zone);
        }
        json.WriteEndArray();

        json.WriteEndObject();
      }

      json.WriteEndArray();
    }

    private class Interval
    {
      public long? Until { get; }
      public string Abbr { get; }
      public double Offset { get; }

      public Interval(long? until, string abbr, double offset)
      {
        Until = until;
        Abbr = abbr;
        Offset = offset;
      }
    }
  }
}
