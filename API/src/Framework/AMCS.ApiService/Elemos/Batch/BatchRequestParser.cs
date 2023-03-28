using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos.Batch
{
  internal static class BatchRequestParser
  {
    public static IList<IBatchRequest> FromStream(Stream stream)
    {
      JArray array;

      using (var reader = new StreamReader(stream))
      using (var json = new JsonTextReader(reader))
      {
        json.DateParseHandling = DateParseHandling.None;
        json.FloatParseHandling = FloatParseHandling.Decimal;

        array = JArray.Load(json);
      }

      try
      {
        return Parse(array);
      }
      catch (BatchRequestException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new BatchRequestException("Invalid batch request", ex);
      }
    }

    private static IList<IBatchRequest> Parse(JArray array)
    {
      var requests = new List<IBatchRequest>();

      foreach (JObject obj in array)
      {
        var type = (string)obj["type"];

        switch (type)
        {
          case "create":
            requests.Add(ParseCreate(obj));
            break;
          case "delete":
            requests.Add(ParseDelete(obj));
            break;
          case "getAssociations":
            requests.Add(ParseGetAssociations(obj));
            break;
          case "getCollection":
            requests.Add(ParseGetCollection(obj));
            break;
          case "getNew":
            requests.Add(ParseGetNew(obj));
            break;
          case "get":
            requests.Add(ParseGet(obj));
            break;
          case "message":
            requests.Add(ParseMessage(obj));
            break;
          case "operation":
            requests.Add(ParseOperation(obj));
            break;
          case "update":
            requests.Add(ParseUpdate(obj));
            break;
          default:
            throw new BatchRequestException($"Unknown batch request type {type}");
        }
      }

      return requests;
    }

    private static IBatchRequest ParseCreate(JObject obj)
    {
      return new CreateRequest((string)obj["collection"], (JObject)obj["data"]);
    }

    private static IBatchRequest ParseDelete(JObject obj)
    {
      return new DeleteRequest((string)obj["collection"], (int)obj["id"]);
    }

    private static IBatchRequest ParseGetAssociations(JObject obj)
    {
      return new GetAssociationsRequest(
        (string)obj["collection"],
        (int)obj["id"],
        (string)obj["associationsCollection"],
        (string)obj["filter"],
        (string)obj["search"],
        (string)obj["order"],
        (int?)obj["max"],
        (int?)obj["page"],
        (bool?)obj["includeDeleted"],
        (string)obj["udf"]);
    }

    private static IBatchRequest ParseGetCollection(JObject obj)
    {
      return new GetCollectionRequest(
        (string)obj["collection"],
        (string)obj["filter"],
        (string)obj["search"],
        (string)obj["order"],
        (int?)obj["max"],
        (int?)obj["page"],
        (bool?)obj["includeCount"],
        (bool?)obj["includeDeleted"],
        (string)obj["udf"]);
    }

    private static IBatchRequest ParseGetNew(JObject obj)
    {
      return new GetNewRequest((string)obj["collection"]);
    }

    private static IBatchRequest ParseGet(JObject obj)
    {
      return new GetRequest(
        (string)obj["collection"],
        (int)obj["id"],
        (string)obj["links"],
        (string)obj["include"],
        (string)obj["expand"],
        (string)obj["udf"]);
    }

    private static IBatchRequest ParseMessage(JObject obj)
    {
      return new MessageRequest((string)obj["name"], (JObject)obj["data"]);
    }

    private static IBatchRequest ParseOperation(JObject obj)
    {
      return new OperationRequest(
        (string)obj["collection"],
        (int)obj["id"],
        (string)obj["operation"],
        (JObject)obj["data"]);
    }

    private static IBatchRequest ParseUpdate(JObject obj)
    {
      return new UpdateRequest((string)obj["collection"], (int)obj["id"], (JObject)obj["data"]);
    }
  }
}
