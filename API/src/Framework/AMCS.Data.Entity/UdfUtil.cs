namespace AMCS.Data.Entity
{
  using System.IO;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  
  public static class UdfUtil
  {
    public static JObject ParseEntity(string response)
    {
      using (var stringReader = new StringReader(response))
      using (var jsonReader = new JsonTextReader(stringReader))
      {
        jsonReader.DateParseHandling = DateParseHandling.None; // I hate that I have to do this
        jsonReader.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
        var jObject = JObject.Load(jsonReader);
        return jObject[UdfConstants.UdfProperty] as JObject;
      }
    }
  }
}