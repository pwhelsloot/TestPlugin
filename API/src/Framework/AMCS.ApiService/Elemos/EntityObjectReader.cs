using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Elemos;
using AMCS.ApiService.Support;
using AMCS.Data;
using Newtonsoft.Json.Linq;
using System.Net;
using AMCS.ApiService.Support.JsonDom;
using AMCS.Data.Entity;
using AMCS.Data.Entity.UserDefinedField;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.SQL;
using AMCS.Data.Server.SQL.Querying;
using AMCS.Data.Server.UserDefinedField;
using AMCS.Data.Support;
using NodaTime;
using AMCS.Encryption;
using DataType = AMCS.PluginData.Data.Metadata.UserDefinedFields.DataType;
using AMCS.Data.Configuration;

namespace AMCS.ApiService.Elemos
{
  internal abstract class EntityObjectReader<TId>
    where TId : struct
  {
    private const int UpdatesPageSize = 200;

    private readonly Type entityType;
    private readonly ISessionToken user;

    protected EntityObjectReader(Type entityType, ISessionToken user)
    {
      this.entityType = entityType;
      this.user = user;
    }

    public abstract EntityObject GetById(IDataSession session, ISessionToken user, Type entityType, TId id);

    protected abstract IJsonDomElement BuildIdResponse(IDataSession session, Type entityType, int? id, Guid? guid = null);

    protected abstract IJsonDomElement BuildStatusResponse(IDataSession session, Type entityType, TId? id, bool isSuccess);

    protected abstract IJsonDomElement BuildCollectionResource(IList<EntityObject> entities, string links, string udf, IApiContext context, IDataSession session);

    protected abstract int GetInternalId(IDataSession session, ISessionToken user, Type entityType, TId id);

    protected abstract EntityLinkBuilder GetLinkBuilder(IApiContext context, EntityObject entity);

    public IJsonDomElement GetById(TId id, string links, string include, string expand, string udf, IApiContext context, IDataSession existingSession = null)
    {
      JsonDomObject response;

      var session = existingSession;
      IDataSessionTransaction transaction = null;

      if (session == null)
      {
        session = BslDataSessionFactory.GetDataSession(user, false);
        transaction = session.CreateTransaction();
      }

      try
      {
        var entity = GetById(session, user, entityType, id);
        response = BuildEntityResponse(entity, links, expand, include, udf, context, session);

        transaction?.Commit();
      }
      catch (BslUserException e)
      {
        return BuildErrorObject(e.Message.Replace(e.MessagePrefix, string.Empty), e);
      }
      catch (EntityRecordNotFoundException e)
      {
        throw context.CreateHttpException(HttpStatusCode.NotFound, e.Message);
      }
      finally
      {
        transaction?.Dispose();
        if (existingSession == null)
          session.Dispose();
      }

      return response;
    }

    protected JsonDomObject BuildEntityResponse(EntityObject entity, string links, string expand, string include, string udf, IApiContext context, IDataSession session)
    {
      var response = new JsonDomObject();

      var entityDom = new JsonDomEntity(entity);

      if (!string.IsNullOrEmpty(udf))
        entityDom.Add(UdfConstants.UdfProperty, SetUdfData(udf, entity, session));

      response.Add("resource", entityDom);

      if (!string.IsNullOrEmpty(links))
        response.Add("links", BuildLinks(links, entity, context));
      
      IJsonDomElement expandElement = null;
      if (!string.IsNullOrEmpty(expand))
        expandElement = BuildExpand(expand, entity, session);

      IJsonDomElement includeElement = null;
      if (!string.IsNullOrEmpty(include))
        includeElement = BuildInclude(include, entity, session);
      
      if (expandElement != null || includeElement != null)
      {
        var extra = new JsonDomObject();
        response.Add("extra", extra);

        if (expandElement != null)
          extra.Add("expand", expandElement);
        if (includeElement != null)
          extra.Add("include", includeElement);
      }

      return response;
    }

    public IJsonDomElement GetNew()
    {
      var response = new JsonDomObject();

      EntityObject entity;

      using (var session = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = session.CreateTransaction())
      {
        entity = session.GetNew(user, entityType);

        transaction.Commit();
      }

      response.Add("resource", new JsonDomEntity(entity));

      return response;
    }

    private IJsonDomElement BuildExpand(string expand, EntityObject entity, IDataSession session)
    {
      // This expands all parent relationships identified by the expand argument.

      var parts = expand.Split(',');
      bool expandAll = parts.Contains("$all");
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForType(entityType);

      JsonDomObject response = null;

      foreach (var relationship in metadata.Relationships)
      {
        if (
          !(relationship is IEntityObjectParentRelationship parentRelationship) ||
          !(expandAll || parts.Contains(parentRelationship.Name)))
          continue;

        int? parentId = (int?)parentRelationship.Property.GetValue(entity);
        if (parentId == null)
          continue;

        var parent = session.GetById(user, parentRelationship.Target, parentId.Value);

        if (response == null)
          response = new JsonDomObject();
        response.Add(parentRelationship.Name, new JsonDomEntity(parent));
      }

      return response;
    }

    private IJsonDomElement BuildInclude(string include, EntityObject entity, IDataSession session)
    {
      // This expands all child relationships identified by the include argument.

      var parts = include.Split(',');
      bool includeAll = parts.Contains("$all");
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForType(entityType);

      JsonDomObject response = null;

      foreach (var relationship in metadata.Relationships)
      {
        if (
          !(relationship is IEntityObjectChildRelationship childRelationship) ||
          !(includeAll || parts.Contains(childRelationship.Name)))
          continue;

        var criteria = Criteria.For(childRelationship.Target)
          .Add(Expression.Eq(childRelationship.TargetProperty.Name, entity.Id));

        var query = BusinessServiceManager.GetService(childRelationship.Target).GetApiCollection(user, criteria, false, session);

        if (response == null)
          response = new JsonDomObject();
        response.Add(childRelationship.Name, new JsonDomEntityArray(query.Entities));
      }

      return response;
    }

    private IJsonDomElement BuildLinks(string links, EntityObject entity, IApiContext context)
    {
      var include = ParseLinks(links);
      var entityLinks = GetLinkBuilder(context, entity);

      var response = new JsonDomObject();

      if (include.HasFlag(Link.Self))
        response.Add("self", JsonDomValue.From(entityLinks.Self));
      if (include.HasFlag(Link.Associations))
        response.Add("associations", BuildArray(entityLinks.GetAssociations()));
      if (include.HasFlag(Link.Expand))
        response.Add("expand", BuildArray(entityLinks.GetExpands()));
      if (include.HasFlag(Link.Operations))
        response.Add("operations", BuildArray(entityLinks.GetOperations()));

      return response;

      IJsonDomElement BuildArray(IList<string> values)
      {
        var array = new JsonDomArray();

        foreach (string value in values)
        {
          array.Add(JsonDomValue.From(value));
        }

        return array;
      }
    }
    
    private static Link ParseLinks(string links)
    {
      var result = Link.None;

      foreach (string part in links.Split(','))
      {
        switch (part)
        {
          case "$all":
            result |= Link.All;
            break;
          case "self":
            result |= Link.Self;
            break;
          case "associations":
            result |= Link.Associations;
            break;
          case "operations":
            result |= Link.Operations;
            break;
          case "expand":
            result |= Link.Expand;
            break;
          default:
            throw new ArgumentException("Unknown link", nameof(links));
        }
      }

      return result;
    }

    public IJsonDomElement GetCollection(string filter, string search, string order, int? max, int? page, bool? includeCount, bool? includeDeleted, string links, string udf, IApiContext context)
    {
      try
      {
        var criteria = ParseFilter(entityType, filter, search, order, max, page, includeDeleted);
        IJsonDomElement response;

        using (var session = BslDataSessionFactory.GetDataSession(user))
        using (var transaction = session.CreateTransaction())
        {
          var query = BusinessServiceManager.GetService(entityType).GetApiCollection(user, criteria, includeCount.GetValueOrDefault(), session);

          response = GetApiQueryResponse(query, links, udf, context, session);

          transaction.Commit();
        }

        return response;
      }
      catch (BslUserException e)
      {
        return BuildErrorObject(e.Message.Replace(e.MessagePrefix, string.Empty), e);
      }
      catch (EntityRecordNotFoundException e)
      {
        throw context.CreateHttpException(HttpStatusCode.NotFound, e.Message);
      }
    }

    public IJsonDomElement GetAssociations(TId id, IEntityObjectChildRelationship relationship, string filter, string search, string order, int? max, int? page, bool? includeDeleted, string links, string udf, IApiContext context)
    {
      IJsonDomElement response;

      using (var session = BslDataSessionFactory.GetDataSession(user))
      using (var transaction = session.CreateTransaction())
      {
        var criteria = ParseFilter(relationship.Target, filter, search, order, max, page, includeDeleted);

        criteria.Add(Expression.Eq(
          relationship.TargetProperty.Name,
          GetInternalId(session, user, entityType, id)));

        var query = BusinessServiceManager.GetService(relationship.Target).GetApiCollection(user, criteria, false, session);

        response = GetApiQueryResponse(query, links, udf, context, session);

        transaction.Commit();
      }

      return response;
    }

    private IJsonDomElement GetApiQueryResponse(ApiQuery query, string links, string udf, IApiContext context, IDataSession session)
    {
      var response = new JsonDomObject();

      response.Add("resource", BuildCollectionResource(query.Entities, links, udf, context, session));

      if (query.Count.HasValue || query.Summary != null)
      { 
        var extra = new JsonDomObject();
        response.Add("extra", extra);
      
        if (query.Count.HasValue)
        {
            extra.Add("count", JsonDomValue.From(query.Count.Value));
        }
        if (query.Summary != null)
        {
            extra.Add("summary", new JsonDomEntity(query.Summary));
        }
      }

      return response;
    }
    
    private static ICriteria ParseFilter(Type type, string filter, string search, string order, int? max, int? page, bool? includeDeleted)
    {
      ICriteria criteria;
      if (filter == null)
        criteria = Criteria.For(type);
      else
        criteria = CriteriaFilterParser.Parse(filter, type);

      if (!string.IsNullOrEmpty(search))
        criteria.Add(Expression.Search(search));

      if (!string.IsNullOrEmpty(order))
      {
        var accessor = EntityObjectAccessor.ForType(type);

        foreach (string part in order.Split(','))
        {
          string fieldName;
          OrderDirection direction = OrderDirection.Ascending;

          if (part.EndsWith(" asc", StringComparison.OrdinalIgnoreCase))
          {
            fieldName = part.Substring(0, part.Length - " asc".Length).TrimEnd();
          }
          else if (part.EndsWith(" desc", StringComparison.OrdinalIgnoreCase))
          {
            fieldName = part.Substring(0, part.Length - " desc".Length).TrimEnd();
            direction = OrderDirection.Descending;
          }
          else
          {
            fieldName = part;
          }

          var property = accessor.GetPropertyByColumnName(fieldName);
          if (property == null)
            throw new ApiException($"Cannot find order by field named '{fieldName}'");

          criteria.Order(property.Name, direction);
        }
      }

      if (max.HasValue)
      {
        criteria.SetMaxResults(max.Value);
        if (page.HasValue)
          criteria.SetFirstResult(page.Value * max.Value);
      }

      if(includeDeleted.HasValue)
      {
        criteria.SetIncludeDeleted(includeDeleted.Value);
      }

      return criteria;
    }

    public IJsonDomElement GetChanges(string since, string cursor, string links, IApiContext context)
    {
      if (since != null && cursor != null)
        throw new ApiException("Either since or cursor can be provided");

      var service = BusinessServiceManager.GetService(entityType) as IEntityObjectChangesService;
      if (service == null)
        throw context.CreateHttpException(HttpStatusCode.NotFound, null);

      var response = new JsonDomObject();
      IList<EntityObject> entities = null;
      ChangesFilter filter;

      using (var session = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = session.CreateTransaction())
      {
        if (cursor != null)
        {
          filter = ChangesFilter.Parse(cursor);
        }
        else
        {
          // string.Empty parses as byte[0]. This should make queries simpler,
          // requiring just RowVersion > @Start and RowVersion <= @End.

          var rowVersionStart = RowVersionUtils.ParseEncryptedRowVersion(since ?? string.Empty);
          var rowVersionEnd = service.GetHighestRowVersion(user, session) ?? new byte[0];

          filter = new ChangesFilter(rowVersionStart, rowVersionEnd, null);
        }

        if (filter.RowVersionEnd.Length > 0)
          entities = service.GetChanges(user, filter, session);

        transaction.Commit();

        response.Add("resource", BuildCollectionResource(entities ?? new List<EntityObject>(), links, null, context, session));
      }

      var extra = new JsonDomObject();
      response.Add("extra", extra);
      var responseLinks = new JsonDomObject();
      response.Add("links", responseLinks);

      if (entities == null || entities.Count < UpdatesPageSize)
      {
        var nextSince = RowVersionUtils.PrintEncryptedRowVersion(filter.RowVersionEnd);

        extra.Add("until", JsonDomValue.From(nextSince));
        responseLinks.Add("next", JsonDomValue.From(BuildLink("since", nextSince)));
      }
      else
      {
        // The GetUpdates contract requires that entities are ordered
        // by ID ascending.

        var lastEntity = entities[entities.Count - 1];
        var nextCursor = filter.Print(lastEntity.Id32);

        extra.Add("cursor", JsonDomValue.From(nextCursor));
        responseLinks.Add("next", JsonDomValue.From(BuildLink("cursor", nextCursor)));
      }

      return response;

      string BuildLink(string parameter, string value)
      {
        var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForType(entityType);
        string serviceRoot = DataServices.Resolve<IServiceRootResolver>().ServiceRoot;

        return
          serviceRoot.TrimEnd('/') + '/' +
          Inflector.Pluralize(metadata.CollectionName) +
          "/changes?" + parameter + "=" + Uri.EscapeDataString(value);
      }
    }

    public virtual IJsonDomElement Create(JObject data)
    {
      using (var session = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = session.CreateTransaction())
      {
        return Save(data, (EntityObject)Activator.CreateInstance(entityType), session, transaction, WriteActionType.Create);
      }
    }

    public virtual IJsonDomElement Update(TId id, JObject data)
    {
      using (var session = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = session.CreateTransaction())
      {
        var entity = GetById(session, user, entityType, id);

        return Save(data, entity, session, transaction, WriteActionType.Update);
      }
    }

    private IJsonDomElement Save(JObject data, EntityObject entity, IDataSession session, IDataSessionTransaction transaction, WriteActionType writeActionType)
    {
      ParseRequestEntity(entity, data, session);
      entity.WriteActionType = writeActionType;

      IJsonDomElement response;

      try
      {
        int? id = session.Save(user, entity);

        if (data[UdfConstants.UdfProperty] != null)
        {
          // We have no guarantee that the GUID passed in from the request or generated in this service won't be overwritten downstream.
          // So to avoid a data mess, make sure to fetch the latest data for the given record
          var updatedEntity = session.GetById(user, entity.GetType(), id.Value);

          // Note: This is an edge-case that should be rare but can happen, e.g., if the underlying SQL for fetching the given entity
          // uses a sproc, but something changed in the initial entity save where the sproc will no longer return the record. We should
          // be explicit here instead of swallowing the issue or throwing a generic null exception.
          if (updatedEntity == null)
            throw new UdfWriteException("Could not find entity after initial save; UDF data write failed");

          WriteUdfData(data[UdfConstants.UdfProperty], updatedEntity.GUID.Value, entity.GetType(), session);
        }

        response = BuildIdResponse(session, entityType, id, entity.GUID);

        transaction.Commit();
      }
      catch (BslUserException e)
      {
        return BuildErrorObject(ApiBslUserExceptionFormatter.FormatError(e.Message.Replace(e.MessagePrefix, string.Empty)), e);
      }

      return response;
    }

    private void ParseRequestEntity(EntityObject entity, JObject data, IDataSession session)
    {
      ParseRequestEntity(entity, entityType, data, session);
    }

    private void ParseRequestEntity(EntityObject entity, Type entityType, JObject data, IDataSession session)
    {
      var accessor = EntityObjectAccessor.ForType(entityType);
      
      foreach (var entry in data)
      {
        // First try to treat the property as an entity blob property. This
        // doesn't follow the normal pattern where the API specifies column
        // names. Instead, it requires the property name.

        var blobProperty = accessor.GetProperty(entry.Key);
        if (blobProperty?.Type == typeof(EntityBlob))
        {
          var blob = (EntityBlob)blobProperty.GetValue(entity);
          byte[] buffer = (byte[])ParseRequestValue(typeof(byte[]), entry.Value, session);

          if (buffer == null)
            BusinessServiceManager.ClearBlob(blob);
          else
            BusinessServiceManager.SaveBlob(blob, buffer);
          continue;
        }

        if (string.Equals(entry.Key, "GUID", StringComparison.OrdinalIgnoreCase) && entry.Value.Type != JTokenType.Null)
          entity.GUID = Guid.Parse((string)entry.Value);

        var property = accessor.GetPropertyByColumnName(entry.Key);
        if (property == null)
        {
          if (entry.Key == UdfConstants.UdfProperty)
            continue;

          throw new ApiException($"Unknown field {entry.Key} on entity {entityType.Name}");
        }

        try
        {
          var value = ParseRequestValue(property.Type, entry.Value, session);
          if (value == null && property.ApiConfiguration.CollapseEmptyObject)
            value = EmptyObjectUtils.CreateEmptyObject(property);

          property.SetValue(entity, value);
        }
        catch (InvalidCastException)
        {
          string entryName = entry.Key;
          Type propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
          string propertyTypeName = JsonUtil.GetJsonSchemaType(propertyType);

          //Handle propertyType that is Date/DateTime type as JsonUtil.GetJsonSchemaType
          //is unable to identify data types such as LocalDate, ZonedDateTime, etc.
          //Otherwise, propertyTypeName will be identified as object
          if (string.Equals(propertyTypeName, "object", StringComparison.OrdinalIgnoreCase))
            propertyTypeName = propertyType.Name;

          throw new ApiException($"The value on {entryName} has the wrong type. Could not process the request." +
            $" Provide a value for {entryName} that is a {propertyTypeName}.");
        }
      }

      if (!entity.GUID.HasValue)
        entity.GUID = SQLGuidComb.Generate();
    }

    private object ParseRequestValue(Type type, JToken token, IDataSession session)
    {
      type = Nullable.GetUnderlyingType(type) ?? type;
      if (type == typeof(ZonedDateTime))
        return ParseZonedDateTime(token);

      if (token is JArray array)
        return ParseRequestArrayValue(type, array, session);
      if (token is JObject obj)
        return ParseRequestObjectValue(type, obj, session);

      object value = ((JValue)token)?.Value;

      if (value == null)
        return null;

      return JsonUtil.Parse(value, Nullable.GetUnderlyingType(type) ?? type);
    }

    private object ParseZonedDateTime(JToken token)
    {
      if (token is JValue value && value.Value == null)
        return null;

      if (token is JObject obj)
      {
        string dateTimeValue = (string)obj["DateTime"];
        string timeZoneId = (string)obj["TimeZone"];

        if (dateTimeValue == null && timeZoneId == null)
          return null;

        var dateTime = TimeZoneUtils.OffsetDateTimePattern.Parse(dateTimeValue).GetValueOrThrow();
        var timeZone = TimeZoneUtils.DateTimeZoneProvider[timeZoneId];

        return dateTime.InZone(timeZone);
      }

      throw new ApiException("Expected null or object as zoned date/time value");
    }

    private object ParseRequestObjectValue(Type type, JObject obj, IDataSession session)
    {
      if (!typeof(EntityObject).IsAssignableFrom(type))
        throw new ApiException("Nested objects must inherit from EntityObject");

      var entityObject = (EntityObject)Activator.CreateInstance(type);

      ParseRequestEntity(entityObject, type, obj, session);

      return entityObject;
    }

    private object ParseRequestArrayValue(Type type, JArray array, IDataSession session)
    {
      if (!typeof(IList<>).IsAssignableFromGeneric(type))
        throw new ApiException("Cannot deserialize JSON array into type " + type);

      var elementType = type.GetGenericTypeArguments(typeof(IList<>))[0];
      var unwrappedType = Nullable.GetUnderlyingType(elementType) ?? elementType;
      var listType = typeof(List<>).MakeGenericType(elementType);
      var list = (IList)Activator.CreateInstance(listType);

      foreach (var value in array)
      {
        var item = ParseRequestValue(elementType, value, session);
        if (!unwrappedType.IsEnum)
          item = ValueCoercion.Coerce(item, unwrappedType);
        list.Add(item);
      }

      return list;
    }

    public IJsonDomElement Delete(TId id)
    {
      IJsonDomElement response;

      try
      {
        using (var session = BslDataSessionFactory.GetDataSession(user))
        using (var transaction = session.CreateTransaction())
        {
          var entity = GetById(session, user, entityType, id);
          session.Delete(user, entity, false);

          if (DataServices.TryResolve<IUdfDataService>(out var udfDataService) && entity.GUID.HasValue)
            udfDataService.Delete(new List<Guid> { entity.GUID.Value }, user, session);

          response = BuildStatusResponse(session, entityType, id, true);

          transaction.Commit();
        }
      }
      catch (BslUserException e)
      {
        return BuildErrorObject(e.Message.Replace(e.MessagePrefix, string.Empty), e);
      }
      catch (EntityRecordNotFoundException e)
      {
        return BuildErrorObject(e.Message, null);
      }

      return response;
    }

    private IJsonDomElement BuildErrorObject(string message, BslUserException exception)
    {
      var response = new JsonDomObject();

      response.Add("errors", JsonDomValue.From(message));

      if (exception?.Errors == null)
        return response;

      var validationErrors = new JsonDomArray();
      response.Add("validationErrors", validationErrors);

      DataServices.TryResolve<IEntityObjectMapper>(out var mapper);

      EntityObjectAccessor accessor = null;
      if (exception.EntityObject != null)
        accessor = EntityObjectAccessor.ForType(exception.EntityObject.GetType());

      foreach (var error in exception.Errors)
      {
        var jsonError = new JsonDomObject();
        validationErrors.Add(jsonError);

        var propertyName = error.PropertyName;
        if (mapper != null && accessor != null)
        {
          var property = accessor.GetProperty(propertyName);
          if (property != null && mapper.TryMapProperty(property, entityType, out var targetProperty))
            propertyName = targetProperty.Name;
        }

        jsonError.Add("property", JsonDomValue.From(propertyName));
        jsonError.Add("message", JsonDomValue.From(error.Message));
      }

      return response;
    }

    protected void SetUdfDataForMultipleEntities(string udfFlag, IList<EntityObject> entities, JsonDomEntityArray existingArray, IDataSession dataSession)
    {
      if (!DataServices.TryResolve<IUdfMetadataService>(out var udfMetadataService)
          || !DataServices.TryResolve<IUdfDataService>(out var udfDataService)
          || entities.Count == 0)
      {
        return;
      }

      var guids = new List<Guid>();
      foreach (var entity in entities)
      {
        guids.Add(entity.GUID.Value);
      }

      var businessService = DataServices.Resolve<IBusinessObjectService>();
      var businessObjectNames = new List<string>();
      foreach (var businessObject in businessService.GetAll())
      {
        if (businessObject.Types.Contains(entities[0].GetType()))
          businessObjectNames.Add(businessObject.BusinessObject.Name);
      }

      if (udfFlag == UdfConstants.UdfAllNamespacesFlag)
        BuildResponseForAllNamespaces();
      else
        BuildResponseForSpecificNamespaces(udfFlag.Split(','));
      
      void BuildResponseForAllNamespaces()
      {
        var udfData = udfDataService.Read(guids, businessObjectNames, user, dataSession);
        foreach (var entity in entities)
        {
          var response = new JsonDomObject();
          foreach (var @namespace in udfMetadataService.GetUdfMetadata().Namespaces)
          {
            if (udfData.TryGetValue(entity.GUID.Value, out var udfResults))
              response.Add(@namespace.Name, BuildObject(@namespace.Name, udfResults));
          }

          existingArray.Add(entity.GUID.Value, (UdfConstants.UdfProperty, response));
        }
      }

      void BuildResponseForSpecificNamespaces(string[] namespaces)
      {
        var udfData = udfDataService.Read(guids, businessObjectNames, namespaces, user, dataSession);
        foreach (var entity in entities)
        {
          var response = new JsonDomObject();
          foreach (var parsedNamespace in namespaces)
          {
            foreach (var @namespace in udfMetadataService.GetUdfMetadata().Namespaces)
            {
              if (@namespace.Name != parsedNamespace)
                continue;

              if (udfData.TryGetValue(entity.GUID.Value, out var udfResults))
                response.Add(@namespace.Name, BuildObject(@namespace.Name, udfResults));
            }
          }

          existingArray.Add(entity.GUID.Value, (UdfConstants.UdfProperty, response));
        }
      }
    }

    protected IJsonDomElement SetUdfData(string udfFlag, EntityObject entity, IDataSession dataSession)
    {
      if (!DataServices.TryResolve<IUdfMetadataService>(out var udfMetadataService)
          || !DataServices.TryResolve<IUdfDataService>(out var udfDataService))
      {
        return null;
      }

      var response = new JsonDomObject();

      var businessService = DataServices.Resolve<IBusinessObjectService>();
      var businessObjectNames = new List<string>();
      foreach (var businessObject in businessService.GetAll())
      {
        if (businessObject.Types.Contains(entity.GetType()))
          businessObjectNames.Add(businessObject.BusinessObject.Name);
      }
      
      if (udfFlag == UdfConstants.UdfAllNamespacesFlag)
        BuildResponseForAllNamespaces();
      else
        BuildResponseForSpecificNamespaces(udfFlag.Split(','));

      return response;

      void BuildResponseForAllNamespaces()
      {
        var udfData = udfDataService.Read(entity.GUID.Value, businessObjectNames, user, dataSession);
        foreach (var @namespace in udfMetadataService.GetUdfMetadata().Namespaces)
        {
          if (udfData.TryGetValue(entity.GUID.Value, out var udfResults))
            response.Add(@namespace.Name, BuildObject(@namespace.Name, udfResults));
        }
      }

      void BuildResponseForSpecificNamespaces(string[] namespaces)
      {
        var udfData = udfDataService.Read(entity.GUID.Value, businessObjectNames, namespaces, user, dataSession);
        foreach (var parsedNamespace in namespaces)
        {
          foreach (var @namespace in udfMetadataService.GetUdfMetadata().Namespaces)
          {
            if (@namespace.Name != parsedNamespace)
              continue;

            if (udfData.TryGetValue(entity.GUID.Value, out var udfResults))
              response.Add(@namespace.Name, BuildObject(@namespace.Name, udfResults));
          }
        }
      }
    }

    private IJsonDomElement BuildObject(string @namespace, Dictionary<IUdfField, IUdfResultObject> udfData)
    {
      var obj = new JsonDomObject();

      foreach (var data in udfData)
      {
        if (data.Key.Namespace == @namespace)
          obj.Add(data.Key.FieldName, SetType(data.Key, data.Value));
      }

      return obj;
    }

    private IJsonDomElement SetType(IUdfField field, IUdfResultObject resultObject)
    {
      switch (field.DataType)
      {
        case DataType.String:
        case DataType.ZonedDateTime:
        case DataType.Duration:
        case DataType.LocalDate:
        case DataType.LocalDateTime:
        case DataType.OffsetDateTime:
        case DataType.LocalTime:
        case DataType.UtcDateTime:
          return string.IsNullOrWhiteSpace(resultObject.StringValue) ? null : JsonDomValue.From(resultObject.StringValue);
        case DataType.Boolean:
          return resultObject.IntegerValue.HasValue ? JsonDomValue.From(resultObject.IntegerValue != 0) : null;
        case DataType.Integer:
          return resultObject.IntegerValue.HasValue ? JsonDomValue.From(resultObject.IntegerValue.Value) : null;
        case DataType.Decimal:
          return resultObject.DecimalValue.HasValue ? JsonDomValue.From(resultObject.DecimalValue.Value) : null;
        case DataType.Text:
          return string.IsNullOrWhiteSpace(resultObject.TextValue) ? null : JsonDomValue.From(resultObject.TextValue);
        default:
          throw new ArgumentOutOfRangeException(nameof(field.DataType));
      }
    }

    private void WriteUdfData(JToken token, Guid entityGuid, Type entityType, IDataSession dataSession)
    {
      if (!DataServices.TryResolve<IUdfDataService>(out var udfDataService))
        return;
      
      var results = new List<(string Namespace, Dictionary<string, object> Items)>();
      
      foreach (var plugin in (JObject) token)
      {
        var udfFields = new Dictionary<string, object>();
        results.Add((plugin.Key, udfFields));

        foreach (var property in (JObject)plugin.Value)
        {
          udfFields.Add(property.Key, property.Value);
        }
      }

      udfDataService.Write(entityGuid, entityType, results, user, dataSession);
    }

    [Flags]
    private enum Link
    {
      None = 0,
      Self = 1,
      Associations = 2,
      Operations = 4,
      Expand = 8,
      All = Self | Associations | Operations | Expand
    }

    private class ChangesFilter : IEntityObjectChangesFilter
    {
      public byte[] RowVersionStart { get; }
      public byte[] RowVersionEnd { get; }
      public int? IdStart { get; }
      public int Count => UpdatesPageSize;

      public ChangesFilter(byte[] rowVersionStart, byte[] rowVersionEnd, int? idStart)
      {
        RowVersionStart = rowVersionStart;
        RowVersionEnd = rowVersionEnd;
        IdStart = idStart;
      }

      public static ChangesFilter Parse(string cursor)
      {
        string decrypted = DataServices.Resolve<IEncryptionService>().Decrypt(cursor);

        var parts = decrypted.Split('|');
        if (parts.Length != 3)
          throw new ArgumentException("Invalid cursor");

        var rowVersionStart = RowVersionUtils.ParseRowVersion(parts[0]);
        var rowVersionEnd = RowVersionUtils.ParseRowVersion(parts[1]);
        int idStart = int.Parse(parts[2], CultureInfo.InvariantCulture);

        return new ChangesFilter(rowVersionStart, rowVersionEnd, idStart);
      }

      public string Print(int idStart)
      {
        var sb = new StringBuilder();

        sb.Append(RowVersionUtils.PrintRowVersion(RowVersionStart));
        sb.Append('|');
        sb.Append(RowVersionUtils.PrintRowVersion(RowVersionEnd));
        sb.Append('|');
        sb.Append(idStart.ToString(CultureInfo.InvariantCulture));

        return DataServices.Resolve<IEncryptionService>().Encrypt(sb.ToString());
      }
    }
  }
}
