using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Elemos
{
  partial class XmlControllerProvider : IControllerProvider
  {
    private static readonly IEntityObjectRelationship[] EmptyList = new IEntityObjectRelationship[0];

    private static readonly XNamespace Ns = "http://amcsgroup.com/Elemos/EntityMetadata/2018-04";

    public static XmlControllerProvider Load(Stream stream)
    {
      var entities = LoadEntities(XDocument.Load(stream));
      var relationships = ParseRelationships(entities);

      return new XmlControllerProvider(entities, relationships);
    }

    private static Dictionary<Type, IList<IEntityObjectRelationship>> ParseRelationships(List<Entity> entities)
    {
      var relationships = new Dictionary<Type, IList<IEntityObjectRelationship>>();

      foreach (var entity in entities)
      {
        var entityRelationships = new List<IEntityObjectRelationship>();
        relationships.Add(entity.Type, new ReadOnlyCollection<IEntityObjectRelationship>(entityRelationships));

        var accessor = EntityObjectAccessor.ForType(entity.Type);

        foreach (var parent in entity.Element.Elements(Ns + "Parent"))
        {
          string targetName = parent.Attribute("Target").Value;
          var target = entities.Single(p => p.Name == targetName && p.RoutePrefix == entity.RoutePrefix).Type;

          entityRelationships.Add(new EntityObjectParentRelationship(
            NamingUtils.CamelCase(parent.Attribute("Name").Value),
            accessor.GetProperty(parent.Attribute("Property").Value),
            target));
        }

        foreach (var child in entity.Element.Elements(Ns + "Child"))
        {
          string targetName = child.Attribute("Target").Value;
          var target = entities.Single(p => p.Name == targetName && p.RoutePrefix == entity.RoutePrefix).Type;
          var targetAccessor = EntityObjectAccessor.ForType(target);

          entityRelationships.Add(new EntityObjectChildRelationship(
            NamingUtils.CamelCase(child.Attribute("Name").Value),
            target,
            targetAccessor.GetProperty(child.Attribute("TargetProperty").Value)));
        }
      }

      return relationships;
    }

    private static List<Entity> LoadEntities(XDocument document)
    {
      var entities = new List<Entity>();

      var enabledApis = new HashSet<string>(
        DataServices.Resolve<IServerConfiguration>().Api.GetEnabledApis().Select(p => p.Name),
        StringComparer.OrdinalIgnoreCase);

      foreach (var context in document.Root.Elements(Ns + "Context"))
      {
        string route = context.Attribute("Route").Value;
        string ns = context.Attribute("Namespace").Value;
        string assembly = context.Attribute("Assembly").Value;
        string contextEnabled = context.Attribute("Enabled")?.Value;

        if (!string.IsNullOrEmpty(contextEnabled) && !enabledApis.Contains(contextEnabled))
          continue;

        foreach (var entity in context.Elements(Ns + "Entity"))
        {
          string entityEnabled = entity.Attribute("Enabled")?.Value;
          if (!string.IsNullOrEmpty(entityEnabled) && !enabledApis.Contains(entityEnabled))
            continue;

          string typeName = ns + "." + entity.Attribute("Type").Value + ", " + assembly;
          var type = Type.GetType(typeName, true);

          var name = entity.Attribute("Name").Value;
          entities.Add(new Entity(name, route, type, entity));
        }
      }

      return entities;
    }

    private List<Entity> entities;
    private readonly Dictionary<Type, IList<IEntityObjectRelationship>> relationships;
    private readonly Dictionary<string, Type> controllerTypes = new Dictionary<string, Type>();

    private XmlControllerProvider(List<Entity> entities, Dictionary<Type, IList<IEntityObjectRelationship>> relationships)
    {
      this.entities = entities;
      this.relationships = relationships;
    }

    public void RegisterRoutes(IRouteCollectionManager routes)
    {
      var messageServices = LoadMessageServices();

      var entityMetadatas = RegisterEntitiesRoutes(routes, messageServices);
      var operations = RegisterServiceRoutes(routes, messageServices);

      ((EntityObjectMetadataManager)DataServices.Resolve<IEntityObjectMetadataManager>()).Initialize(entityMetadatas, operations);
    }

    private MessageServices LoadMessageServices()
    {
      var services = new MessageServices();

      foreach (var type in GetAllTypes())
      {
        if (!type.CanConstruct())
          continue;

        if (typeof(IMessageService<,>).IsAssignableFromGeneric(type))
        {
          var typeArguments = type.GetGenericTypeArguments(typeof(IMessageService<,>));
          var requestType = typeArguments[0];
          var responseType = typeArguments[1];

          var controllerType = typeof(MessageServiceController<,,>).MakeGenericType(type, requestType, responseType);
          services.Services.Add(new MessageService(GetMessageServiceRoute(type), type, controllerType, responseType));
        }
        else if (typeof(IAsyncMessageService<,>).IsAssignableFromGeneric(type))
        {
          var typeArguments = type.GetGenericTypeArguments(typeof(IAsyncMessageService<,>));
          var requestType = typeArguments[0];
          var responseType = typeArguments[1];

          var controllerType = typeof(AsyncMessageServiceController<,,>).MakeGenericType(type, requestType, responseType);
          services.Services.Add(new MessageService(GetMessageServiceRoute(type), type, controllerType, responseType));
        }
        else if (typeof(IEntityObjectMessageService<,,>).IsAssignableFromGeneric(type))
        {
          var typeArguments = type.GetGenericTypeArguments(typeof(IEntityObjectMessageService<,,>));
          var entityType = typeArguments[0];
          var requestType = typeArguments[1];
          var responseType = typeArguments[2];

          var controllerType = typeof(EntityObjectMessageServiceController<,,,>).MakeGenericType(type, entityType, requestType, responseType);

          if (!services.EntityServices.TryGetValue(entityType, out var entityServices))
          {
            entityServices = new List<MessageService>();
            services.EntityServices.Add(entityType, entityServices);
          }

          entityServices.Add(new MessageService(GetMessageServiceRoute(type), type, controllerType, responseType));
        }
        else if (typeof(IAsyncMessageService<,>).IsAssignableFromGeneric(type))
        {
          var typeArguments = type.GetGenericTypeArguments(typeof(IAsyncMessageService<,>));
          var requestType = typeArguments[0];
          var responseType = typeArguments[1];

          var controllerType = typeof(AsyncMessageServiceController<,,>).MakeGenericType(type, requestType, responseType);
          services.Services.Add(new MessageService(GetMessageServiceRoute(type), type, controllerType, responseType));
        }
        else if (typeof(IEntityObjectAsyncMessageService<,,>).IsAssignableFromGeneric(type))
        {
          var typeArguments = type.GetGenericTypeArguments(typeof(IEntityObjectAsyncMessageService<,,>));
          var entityType = typeArguments[0];
          var requestType = typeArguments[1];
          var responseType = typeArguments[2];

          var controllerType = typeof(EntityObjectAsyncMessageServiceController<,,,>).MakeGenericType(type, entityType, requestType, responseType);

          if (!services.EntityServices.TryGetValue(entityType, out var entityServices))
          {
            entityServices = new List<MessageService>();
            services.EntityServices.Add(entityType, entityServices);
          }

          entityServices.Add(new MessageService(GetMessageServiceRoute(type), type, controllerType, responseType));
        }
      }

      return services;
    }

    private string GetMessageServiceRoute(Type type)
    {
      var attributes = type.GetCustomAttributes(typeof(ServiceRouteAttribute), true);
      if (attributes.Length != 1)
        throw new InvalidOperationException("Service type requires a ServiceRoute attribute");

      return ((ServiceRouteAttribute)attributes[0]).Route;
    }

    private IEnumerable<Type> GetAllTypes()
    {
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (assembly.IsDynamic)
          continue;

        Type[] types;

        try
        {
          types = assembly.GetExportedTypes();
        }
        catch
        {
          // This may fail, but not on assemblies we need it to succeed.
          continue;
        }

        foreach (var type in types)
        {
          yield return type;
        }
      }
    }

    private IEnumerable<IOperation> RegisterServiceRoutes(IRouteCollectionManager routes, MessageServices services)
    {
      var operations = new List<IOperation>();

      foreach (var service in services.Services)
      {
        routes.MapServiceRoute(
          service.Type.Name,
          service.Route,
          service.ControllerType,
          service.ResponseEntityType);

        controllerTypes.Add(service.Type.Name, service.ControllerType);

        operations.Add(new Operation(service.Route, service.Type));
      }

      return operations;
    }

    private List<IEntityObjectMetadata> RegisterEntitiesRoutes(IRouteCollectionManager routes, MessageServices services)
    {
      var metadatas = new List<IEntityObjectMetadata>();

      foreach (var entity in entities)
      {
        RegisterEntityRoutes(routes, metadatas, entity, services);
      }

      // We don't need entities anymore. Since entities is holding on to XElement's,
      // it makes sense to drop this whole thing.
      entities = null;

      return metadatas;
    }

    private void RegisterEntityRoutes(IRouteCollectionManager routes, List<IEntityObjectMetadata> metadatas, Entity entity, MessageServices services)
    {
      string controllerName = entity.Name + "Controller";

      for (int i = 1; ; i++)
      {
        if (!controllerTypes.ContainsKey(controllerName))
          break;
        controllerName = entity.Name + "Controller_" + i;
      }

      var attribute = entity.Type.GetCustomAttribute<ApiExplorerAttribute>();
      var mode = attribute?.Mode ?? ApiMode.Internal;
      var controllerType = mode == ApiMode.Internal
        ? typeof(EntityObjectServiceInternalController<>)
        : typeof(EntityObjectServiceExternalController<>);
      var entityControllerType = controllerType.MakeGenericType(entity.Type);

      controllerTypes.Add(controllerName, entityControllerType);

      string objectName = NamingUtils.CamelCase(entity.Name);
      string collectionName = NamingUtils.CamelCase(Inflector.Pluralize(entity.Name));

      string routePrefix = entity.RoutePrefix;
      if (routePrefix != null)
      {
        routePrefix = routePrefix.Trim('/');
        if (routePrefix.Length > 0)
        {
          objectName = routePrefix + "/" + objectName;
          collectionName = routePrefix + "/" + collectionName;
        }
      }

      var relationships = GetRelationships(entity.Type);

      var builder = routes.GetRouteBuilder(controllerName, collectionName, entityControllerType);

      var entityType = typeof(ApiEntityDescription<>).MakeGenericType(entity.Type);
      var resourceType = typeof(ApiResourceResultDescription<>).MakeGenericType(entityType);
      var resourceEntityType = typeof(ApiResourceResultEntityDescription<>).MakeGenericType(entityType);
      var resourceCollectionType = typeof(ApiResourceResultCollectionDescription<>).MakeGenericType(entityType);
      var resourceChangesType = typeof(ApiResourceResultChangesDescription<>).MakeGenericType(entityType);

      // The order of these rules is important. The {id} rules are catch all rules,
      // so specific ones need to appear before them.

      builder.MapRoute("GetChanges", "changes", "GET", resourceChangesType);
      builder.MapRoute("GetNew", "template", "GET", resourceType);
      builder.MapRoute("Get", "{id}", "GET", resourceEntityType);

      builder.MapRoute("GetBlob", "{id}/blob/{blobMemberName}", "GET", FileStreamResultType);

      builder.MapMvcRoute("GetAssociations", "{id}/{associatedCollectionName}", "GET");

      foreach (var relationship in relationships.OfType<IEntityObjectChildRelationship>())
      {
        var associationsEntityType = typeof(ApiEntityDescription<>).MakeGenericType(relationship.Target);
        var associationsCollectionType = typeof(ApiResourceResultCollectionDescription<>).MakeGenericType(associationsEntityType);
        builder.MapExplorerRoute($"Get{relationship.Name}Associations", "GetAssociations", "{id}/" + NamingUtils.CamelCase(relationship.Name), "GET", associationsCollectionType);
      }

      builder.MapRoute("GetCollection", null, "GET", resourceCollectionType);
      builder.MapRoute("Create", null, "POST", typeof(ApiResourceIdDescription));
      builder.MapRoute("Update", "{id}", "PUT", typeof(ApiResourceIdDescription));
      builder.MapRoute("Delete", "{id}", "DELETE", typeof(ApiResourceDescription));

      // Register the service routes.

      var operations = new List<IOperation>();

      if (services.EntityServices.TryGetValue(entity.Type, out var entityServices))
      {
        foreach (var service in entityServices)
        {
          var operationRoute = service.Route.TrimStart('/');

          operations.Add(new Operation(operationRoute, service.Type));

          string route = collectionName.Trim('/') + "/{id}/op/" + operationRoute;

          routes.MapServiceRoute(
            service.Type.Name,
            route,
            service.ControllerType,
            service.ResponseEntityType);

          controllerTypes.Add(service.Type.Name, service.ControllerType);
        }
      }

      metadatas.Add(new EntityObjectMetadata(
        entity.Type,
        objectName,
        collectionName,
        relationships,
        new ReadOnlyCollection<IOperation>(operations)));
    }

    private IList<IEntityObjectRelationship> GetRelationships(Type entityType)
    {
      relationships.TryGetValue(entityType, out var result);
      return result ?? EmptyList;
    }

    public List<string> GetControllerNames()
    {
      return controllerTypes.Keys.ToList();
    }

    public Type GetControllerType(string controllerName)
    {
      controllerTypes.TryGetValue(controllerName, out var type);
      return type;
    }

    private class Entity
    {
      public string Name { get; }

      public string RoutePrefix { get; }

      public Type Type { get; }

      public XElement Element { get; }

      public Entity(string name, string routePrefix, Type type, XElement element)
      {
        Name = name;
        RoutePrefix = routePrefix;
        Type = type;
        Element = element;
      }
    }

    private class EntityObjectParentRelationship : IEntityObjectParentRelationship
    {
      public string Name { get; }

      public EntityObjectRelationshipKind Kind => EntityObjectRelationshipKind.Parent;

      public EntityObjectProperty Property { get; }

      public Type Target { get; }

      public EntityObjectParentRelationship(string name, EntityObjectProperty property, Type target)
      {
        Name = name;
        Property = property;
        Target = target;
      }
    }

    private class EntityObjectChildRelationship : IEntityObjectChildRelationship
    {
      public string Name { get; }

      public EntityObjectRelationshipKind Kind => EntityObjectRelationshipKind.Child;

      public Type Target { get; }

      public EntityObjectProperty TargetProperty { get; }

      public EntityObjectChildRelationship(string name, Type target, EntityObjectProperty targetProperty)
      {
        Name = name;
        Target = target;
        TargetProperty = targetProperty;
      }
    }

    private class EntityObjectMetadata : IEntityObjectMetadata
    {
      public Type EntityType { get; }

      public string ObjectName { get; }

      public string CollectionName { get; }

      public IList<IEntityObjectRelationship> Relationships { get; }

      public IList<IOperation> Operations { get; }

      public EntityObjectMetadata(Type entityType, string objectName, string collectionName, IList<IEntityObjectRelationship> relationships, IList<IOperation> operations)
      {
        EntityType = entityType;
        ObjectName = objectName;
        CollectionName = collectionName;
        Relationships = relationships;
        Operations = operations;
      }
    }

    private class Operation : IOperation
    {
      public string Name { get; }

      public Type Handler { get; }

      public Operation(string name, Type handler)
      {
        Name = name;
        Handler = handler;
      }
    }

    private class MessageServices
    {
      public Dictionary<Type, List<MessageService>> EntityServices { get; } = new Dictionary<Type, List<MessageService>>();

      public List<MessageService> Services { get; } = new List<MessageService>();
    }

    private class MessageService
    {
      public string Route { get; }

      public Type Type { get; }

      public Type ControllerType { get; }

      public Type ResponseEntityType { get; }

      public MessageService(string route, Type type, Type controllerType, Type responseEntityType)
      {
        Route = route;
        Type = type;
        ControllerType = controllerType;
        ResponseEntityType = responseEntityType;
      }
    }
  }
}
