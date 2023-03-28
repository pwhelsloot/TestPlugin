using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.PlatformFramework.CommsServer.Messages
{
  public static class MessageSerializer
  {
    private static readonly Lazy<Dictionary<string, Type>> types = new Lazy<Dictionary<string, Type>>(CreateMessageTypes, LazyThreadSafetyMode.None);

    private static Dictionary<string, Type> CreateMessageTypes()
    {
      var types = new Dictionary<string, Type>();

      //
      // Find all types that:
      //
      // * Implement IMessage;
      // * Are a class;
      // * Are not abstract;
      // * Have a public constructor without parameters.
      //

      foreach (var type in typeof(IMessage).Assembly.GetTypes())
      {
        if (
          typeof(IMessage).IsAssignableFrom(type) &&
          type.IsClass &&
          !type.IsAbstract &&
          type.GetConstructors().Any(p => p.IsPublic && p.GetParameters().Length == 0)
        )
          types.Add(type.Name, type);
      }

      return types;
    }

    public static string GetMessageType(IMessage message)
    {
      return message.GetType().Name;
    }

    public static string Serialize(IMessage message)
    {
      return JsonConvert.SerializeObject(message);
    }

    public static IMessage Deserialize(string type, string body)
    {
      return (IMessage)JsonConvert.DeserializeObject(body, types.Value[type]);
    }
  }
}
