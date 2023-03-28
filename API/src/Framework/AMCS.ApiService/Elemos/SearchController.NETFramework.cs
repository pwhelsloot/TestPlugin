#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Support;
using AMCS.ApiService.Support.JsonDom;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos
{
  [Authenticated]
  [Route("search")]
  public class SearchController : Controller
  {
    private const int MaxResults = 25;

    [HttpPost]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "C# 7 language construct")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "C# 7 language construct")]
    public ActionResult Search()
    {
      JObject obj;

      using (var reader = new StreamReader(HttpContext.Request.InputStream))
      using (var json = new JsonTextReader(reader))
      {
        obj = JObject.Load(json);
      }

      var request = ParseRequest(obj);

      int maxResults = Math.Min(request.Max.GetValueOrDefault(MaxResults), MaxResults);
      int firstResult = (request.Page.GetValueOrDefault() * maxResults) + 1;

      var tables = new List<string>();
      var tableMap = new Dictionary<string, (EntityObjectAccessor, Collection)>(StringComparer.OrdinalIgnoreCase);

      foreach (var collection in request.Collections)
      {
        var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collection.CollectionName);
        if (metadata == null)
          throw new ApiException($"Unknown collection '{collection.CollectionName}'");

        var accessor = EntityObjectAccessor.ForType(metadata.EntityType);
        tables.Add(accessor.TableNameWithSchema);
        tableMap.Add(accessor.TableNameWithSchema, (accessor, collection));
      }

      var sql = new SQLTextBuilder();

      sql
        .Text("SELECT ").Name("Table").Text(", ").Name("TableId").Text(Environment.NewLine)
        .Text("FROM ").TableName("search", "Index").Text(" AS ").Name("Index").Text(Environment.NewLine)
        .Text("INNER JOIN FREETEXTTABLE(").TableName("search", "Index").Text(", ").Name("Text").Text(", ").ParameterName("Search").Text(") ")
        .Text("AS ").Name("IndexKey").Text(" ON ").Name("Index").Text(".").Name("Id").Text(" = ").Name("IndexKey").Text(".").Name("KEY").Text(Environment.NewLine)
        .Text("WHERE ").Name("Index").Text(".").Name("Table").Text(" IN ").ParameterLiteral("Tables").Text(Environment.NewLine)
        .Text("ORDER BY ").Name("IndexKey").Text(".").Name("RANK").Text(" DESC").Text(Environment.NewLine)
        .Text("OFFSET ").ParameterName("FirstResult").Text(" ROWS FETCH FIRST ").ParameterName("MaxResults").Text(" ROWS ONLY");

      var user = HttpContext.GetAuthenticatedUser();

      ContentResult content;

      using (var session = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = session.CreateTransaction())
      {
        var results = session.Query(sql.ToString())
          .Set("@Search", request.Query)
          .SetObject("@Tables", tables)
          .Set("@FirstResult", firstResult)
          .Set("@MaxResults", maxResults)
          .Execute()
          .List(p => (Table: p.Get<string>("Table"), Id: p.Get<int>("TableId")));

        using (var writer = new StringWriter())
        {
          using (var json = new JsonTextWriter(writer))
          {
            json.WriteStartArray();

            foreach (var result in results)
            {
              var (accessor, collection) = tableMap[result.Table];

              var reader = new EntityObjectInternalReader(accessor.Type, user);

              var response = reader.GetById(result.Id, null, collection.Include, collection.Expand, request.Udf, new ApiContext(HttpContext), session);

              ((JsonDomObject)response).Add("resourceType", JsonDomValue.From(collection.CollectionName));

              response.Write(json);
            }

            json.WriteEndArray();
          }

          content = Content(writer.ToString(), "application/json");
        }

        transaction.Commit();
      }

      return content;
    }

    private SearchRequest ParseRequest(JObject obj)
    {
      var request = new SearchRequest(
        (string)obj["query"],
        (int?)obj["max"],
        (int?)obj["page"],
        (string)obj["udf"]);

      foreach (JObject collection in (JArray)obj["collections"])
      {
        request.Collections.Add(new Collection(
          (string)collection["collection"],
          (string)collection["include"],
          (string)collection["expand"]));
      }

      return request;
    }

    private class SearchRequest
    {
      public string Query { get; }

      public int? Max { get; }

      public int? Page { get; }

      public string Udf { get; }

      public List<Collection> Collections { get; } = new List<Collection>();

      public SearchRequest(string query, int? max, int? page, string udf)
      {
        Query = query;
        Max = max;
        Page = page;
        Udf = udf;
      }
    }

    private class Collection
    {
      public string CollectionName { get; }

      public string Include { get; }

      public string Expand { get; }

      public Collection(string collectionName, string include, string expand)
      {
        CollectionName = collectionName;
        Include = include;
        Expand = expand;
      }
    }
  }
}

#endif
