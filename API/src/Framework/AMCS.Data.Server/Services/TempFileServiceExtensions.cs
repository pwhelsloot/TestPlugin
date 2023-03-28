using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.Services
{
  public static class TempFileServiceExtensions
  {
    public static string WriteString(this ITempFileService self, string content)
    {
      return self.WriteBytes(Encoding.UTF8.GetBytes(content));
    }

    public static string WriteBytes(this ITempFileService self, byte[] content)
    {
      using (var stream = new MemoryStream(content))
      {
        return self.WriteFile(stream);
      }
    }

    public static string WriteJson(this ITempFileService self, object value, JsonSerializerSettings settings = null)
    {
      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
        using (var json = new JsonTextWriter(writer))
        {
          JsonSerializer.Create(settings).Serialize(json, value);
        }

        stream.Position = 0;

        return self.WriteFile(stream);
      }
    }

    public static string ReadString(this ITempFileService self, string key)
    {
      using (var stream = self.ReadFile(key))
      using (var reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }

    public static byte[] ReadBytes(this ITempFileService self, string key)
    {
      using (var source = self.ReadFile(key))
      using (var target = new MemoryStream())
      {
        source.CopyTo(target);

        return target.ToArray();
      }
    }

    public static T ReadJson<T>(this ITempFileService self, string key, JsonSerializerSettings settings = null)
    {
      using (var stream = self.ReadFile(key))
      using (var reader = new StreamReader(stream))
      using (var json = new JsonTextReader(reader))
      {
        return JsonSerializer.Create(settings).Deserialize<T>(json);
      }
    }
  }
}
