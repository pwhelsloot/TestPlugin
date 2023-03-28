namespace AMCS.Data.Server.UserDefinedField
{
  using System.IO;
  using Newtonsoft.Json;

  internal class UdfJsonUtil
  {
    internal static object GetSingleUdfRestrictionValue(string metadata)
    {
      using (var stringReader = new StringReader(metadata))
      using (var jsonReader = new JsonTextReader(stringReader))
      {
        var foundValueProperty = false;

        while (jsonReader.Read())
        {
          if (foundValueProperty)
            return jsonReader.Value;

          foundValueProperty =
            jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value.ToString() == "Value";
        }
      }

      return null;
    }
  }
}