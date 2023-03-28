using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AMCS.Data.Configuration.TimeZones.Moment;
using DiffMatchPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.TimeZones;

namespace AMCS.ApiService.Tests.Moment
{
  [TestClass]
  public class MomentTimeZoneDatabaseConverterFixture
  {
    [TestMethod]
    public void Version2019b()
    {
      Test("2019b", DatabaseConverterOptions.TrimIntervals);
    }

    [TestMethod]
    public void Version2019c()
    {
      // Starting with 2019c, moment timezone includes country codes
      // in the time zone database.

      Test("2019c", DatabaseConverterOptions.TrimIntervals | DatabaseConverterOptions.IncludeCountries);
    }

    private void Test(string version, DatabaseConverterOptions options = DatabaseConverterOptions.None)
    {
      TzdbDateTimeZoneSource source;

      using (var stream = GetResource($"tzdb{version}.nzd"))
      {
        source = TzdbDateTimeZoneSource.FromStream(stream);
      }

      string actual;

      using (var stream = DatabaseConverter.GetPopulation())
      {
        actual = DatabaseConverter.ToMoment(source, stream, options);
      }

      string expected = GetStringResource($"{version}.json");

      //
      // There are a few differences in the actual and expected outputs
      // which are difficult to get rid off:
      //
      // * The ordering of the elements aren't exactly correct. We fix
      //   those by changing expected to have it the same order as actual;
      // * Even with that corrected, there are a few differences that
      //   we can't get rid of. To fix that, we include a patch file and
      //   instead of asserting on the outputs, we assert on the expected
      //   and actual patch.
      //
      // Testing the results like this allows us to keep the exact version
      // of moment timezone project taken from
      // https://github.com/moment/moment-timezone/tree/develop/data/unpacked.
      //

      expected = ReorderExpected(expected);

      expected = Prettify(expected);
      actual = Prettify(actual);

      string expectedPatch = SimplifyPatch(GetStringResource($"{version}.patch"));
      string actualPatch = SimplifyPatch(BuildPatch(expected, actual));

      Assert.AreEqual(expectedPatch, actualPatch);
    }

    private string BuildPatch(string expected, string actual)
    {
      var dmp = DiffMatchPatchModule.Default;

      return dmp.PatchToText(dmp.PatchMake(expected, actual));
    }

    private string ReorderExpected(string expected)
    {
      // Reorder the root zones and zones in nested countries.

      var obj = JObject.Parse(expected);

      var zones = obj["zones"].ToArray();

      var zonesArray = (JArray)obj["zones"];
      zonesArray.Clear();

      foreach (JObject zone in zones.OrderBy(p => (string)p["name"]))
      {
        zonesArray.Add(zone);
      }

      if (obj["countries"] != null)
      {
        foreach (var country in (JArray)obj["countries"])
        {
          zones = country["zones"].ToArray();

          zonesArray = (JArray)country["zones"];
          zonesArray.Clear();

          foreach (var zone in zones.OrderBy(p => (string)p))
          {
            zonesArray.Add(zone);
          }
        }
      }

      return obj.ToString();
    }

    private Stream GetResource(string name)
    {
      return GetType().Assembly.GetManifestResourceStream(GetType().Namespace + "." + name);
    }

    private string GetStringResource(string name)
    {
      using (var stream = GetResource(name))
      using (var reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }

    private string Prettify(string json, Formatting formatting = Formatting.Indented)
    {
      using (var reader = new StringReader(json))
      using (var jsonReader = new JsonTextReader(reader))
      using (var writer = new StringWriter())
      {
        using (var jsonWriter = new JsonTextWriter(writer))
        {
          jsonWriter.Formatting = formatting;
          jsonWriter.WriteToken(jsonReader);
        }

        return writer.ToString();
      }
    }

    private string SimplifyPatch(string patch)
    {
      return Regex.Replace(patch, "\r?\n", "\r\n").TrimEnd();
    }
  }
}
