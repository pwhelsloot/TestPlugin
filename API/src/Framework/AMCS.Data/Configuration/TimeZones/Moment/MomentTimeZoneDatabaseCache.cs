using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;
using log4net;
using NodaTime.TimeZones;

namespace AMCS.Data.Configuration.TimeZones.Moment
{
  internal class MomentTimeZoneDatabaseCache : IMomentTimeZoneDatabaseCache
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(MomentTimeZoneDatabaseCache));

    private volatile MomentTimeZoneDatabase momentTimeZoneDatabase;

    public MomentTimeZoneDatabase MomentTimeZoneDatabase => momentTimeZoneDatabase;

    public MomentTimeZoneDatabaseCache(IDateTimeZoneProviderProvider providerProvider)
    {
      providerProvider.DateTimeZoneProviderChanged += ProviderProvider_DateTimeZoneProviderChanged;

      GenerateDatabase(providerProvider.DateTimeZoneSource);
    }

    private void ProviderProvider_DateTimeZoneProviderChanged(object sender, EventArgs e)
    {
      GenerateDatabase(((IDateTimeZoneProviderProvider)sender).DateTimeZoneSource);
    }

    private void GenerateDatabase(TzdbDateTimeZoneSource dateTimeZoneSource)
    {
      try
      {
        string json;

        using (var stream = DatabaseConverter.GetPopulation())
        {
          json = DatabaseConverter.ToMoment(dateTimeZoneSource, stream, DatabaseConverterOptions.IncludeCountries | DatabaseConverterOptions.TrimIntervals);
        }

        var uncompressed = Encoding.UTF8.GetBytes(json);
        var compressed = Compress(uncompressed);
        string hash = GetHash(uncompressed);

        momentTimeZoneDatabase = new MomentTimeZoneDatabase(hash, uncompressed, compressed);
      }
      catch (Exception ex)
      {
        Log.Error("Failed to generate moment timezone database", ex);
      }
    }

    private static byte[] Compress(byte[] uncompressed)
    {
      using (var target = new MemoryStream())
      using (var gzTarget = new GZipStream(target, CompressionLevel.Optimal))
      {
        using (var source = new MemoryStream(uncompressed))
        {
          source.CopyTo(gzTarget);
        }

        return target.ToArray();
      }
    }

    private string GetHash(byte[] data)
    {
      byte[] hash;

      using (var source = new MemoryStream(data))
      using (var sha1 = new SHA1Managed())
      {
        hash = sha1.ComputeHash(source);
      }

      return Escaping.HexEncode(hash);
    }
  }
}
