using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration.TimeZones.Moment
{
  public class MomentTimeZoneDatabase
  {
    private readonly byte[] uncompressed;
    private readonly byte[] compressed;

    public string ETag { get; }

    public MomentTimeZoneDatabase(string eTag, byte[] uncompressed, byte[] compressed)
    {
      this.uncompressed = uncompressed;
      this.compressed = compressed;

      ETag = eTag;
    }

    public Stream GetStream(bool compress)
    {
      return new MemoryStream(compress ? compressed : uncompressed, writable: false);
    }
  }
}
