using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class MapProvidersConfiguration : IPlatformUIMapProvidersConfiguration
  {
    private readonly IPlatformUIMapProvidersConfiguration wrapped;

    public MapProvidersConfiguration(IPlatformUIMapProvidersConfiguration wrapped)
    {
      this.wrapped = wrapped;
    }

    public string GoogleMapsUrl => wrapped.GoogleMapsUrl;

    public string GoogleMapsSatelliteUrl => wrapped.GoogleMapsSatelliteUrl;
    
    public string GoogleMapsTerrainUrl => wrapped.GoogleMapsTerrainUrl;

    public string HereMapsUrl => wrapped.HereMapsUrl;

    public string HereMapsSatelliteUrl => wrapped.HereMapsSatelliteUrl;

    public string HereMapsTerrainUrl => wrapped.HereMapsTerrainUrl;
    
    public string MapDisplayProvider => wrapped.MapDisplayProvider;
  }
}
