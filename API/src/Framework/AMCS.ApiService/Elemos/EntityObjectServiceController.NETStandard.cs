#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Support;
using AMCS.ApiService.Support.JsonDom;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos
{
  [ApiAuthorize]
  public abstract class EntityObjectServiceController<TEntity, TId> : Controller
    where TEntity : EntityObject, new()
    where TId : struct
  {
    internal abstract EntityObjectReader<TId> GetReader();

    [HttpGet]
    public ActionResult Get(TId id, string links, string include, string expand, string udf)
    {
      var result = GetReader().GetById(id, links, include, expand, udf, new ApiContext(HttpContext));

      return JsonResult(result);
    }

    [HttpGet]
    public ActionResult GetBlob(TId id, string blobMemberName, string fileName = null, string contentType = null)
    {
      var userId = HttpContext.GetAuthenticatedUser();

      using (var session = BslDataSessionFactory.GetDataSession(userId, false))
      {
        session.StartTransaction();

        try
        {
          var entity = GetReader().GetById(session, userId, typeof(TEntity), id);

          var property = EntityObjectAccessor.ForType(entity.GetType()).GetProperty(blobMemberName);
          if (property == null)
            throw new ApiException($"No property defined name '{blobMemberName}'");

          var entityBlob = (EntityBlob)property.GetValue(entity);
          var stream = session.GetBlob(entityBlob);

          session.CommitTransaction();

          return new FileStreamResult(stream, contentType ?? "application/octet-stream")
          {
            FileDownloadName = fileName
          };
        }
        catch
        {
          session.RollbackTransaction();
          throw;
        }
      }
    }

    [HttpGet]
    public ActionResult GetAssociations(TId id, string associatedCollectionName, string filter, string search, string order, int? max, int? page, bool? includeDeleted, string links, string udf)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForType(typeof(TEntity));

      foreach (var relationship in metadata.Relationships)
      {
        if (
          relationship is IEntityObjectChildRelationship childRelationship &&
          string.Equals(childRelationship.Name, associatedCollectionName, StringComparison.OrdinalIgnoreCase))
        {
          var result = GetReader().GetAssociations(id, childRelationship, filter, search, order, max, page, includeDeleted, links, udf, new ApiContext(HttpContext));

          return JsonResult(result);
        }
      }

      return NotFound();
    }

    [HttpGet]
    public ActionResult GetCollection(string filter, string search, string order, int? max, int? page, bool? includeCount, bool? includeDeleted, string links, string udf)
    {
      var result = GetReader().GetCollection(filter, search, order, max, page, includeCount, includeDeleted, links, udf, new ApiContext(HttpContext));

      return JsonResult(result);
    }

    [HttpGet]
    public ActionResult GetChanges(string since, string cursor, string links)
    {
      var result = GetReader().GetChanges(since, cursor, links, new ApiContext(HttpContext));

      return JsonResult(result);
    }

    [HttpGet]
    public ActionResult GetNew()
    {
      var result = GetReader().GetNew();

      return JsonResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create()
    {
      var result = GetReader().Create(await ParseSaveRequestAsync());

      return JsonResult(result);
    }

    [HttpPut]
    public async Task<ActionResult> Update(TId id)
    {
      var result = GetReader().Update(id, await ParseSaveRequestAsync());

      return JsonResult(result);
    }

    private async Task<JObject> ParseSaveRequestAsync()
    {
      string content;

      using (var stream = Request.Body)
      using (var reader = new StreamReader(stream))
      {
        content = await reader.ReadToEndAsync();
      }

      using (var reader = new StringReader(content))
      using (var json = new JsonTextReader(reader))
      {
        json.DateParseHandling = DateParseHandling.None;
        json.FloatParseHandling = FloatParseHandling.Decimal;

        return JObject.Load(json);
      }
    }

    [HttpDelete]
    public ActionResult Delete(TId id)
    {
      var result = GetReader().Delete(id);

      return JsonResult(result);
    }

    private ActionResult JsonResult(IJsonDomElement element)
    {
      return new JsonDomResult(element);
    }

    private ActionResult JsonResult(string json)
    {
      return Content(json, "application/json");
    }

    protected void ValidateEntityRoleAccess(Type entityType)
    {
      var roles = HttpContext.GetPlatformCredentials()?.Roles;
      var apiExplorerAttribute = entityType.GetCustomAttribute<ApiExplorerAttribute>();
      if (roles != null && !string.IsNullOrWhiteSpace(apiExplorerAttribute?.UserRole) && !roles.Contains(apiExplorerAttribute.UserRole))
        throw new UnauthorizedAccessException("User does not have sufficient access");
    }

    private class JsonDomResult : ActionResult
    {
      private readonly IJsonDomElement element;

      public JsonDomResult(IJsonDomElement element)
      {
        this.element = element;
      }

      public override async Task ExecuteResultAsync(ActionContext context)
      {
        var response = context.HttpContext.Response;

        var mediaType = new MediaTypeHeaderValue("application/json")
        {
          Encoding = Encoding.UTF8
        };

        response.ContentType = mediaType.ToString();

        using (var stream = new MemoryStream())
        {
          using (var streamWriter = new StreamWriter(stream, leaveOpen: true))
          using (var json = new JsonTextWriter(streamWriter))
          {
            element.Write(json);
          }

          stream.Position = 0;

          await stream.CopyToAsync(response.Body);
        }
      }
    }
  }
}

#endif
