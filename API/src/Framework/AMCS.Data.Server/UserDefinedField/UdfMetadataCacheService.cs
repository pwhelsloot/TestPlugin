namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.SQL.Querying;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;

  internal class UdfMetadataCacheService : CacheCoherentEntityService<UdfMetadataEntity>, IUdfMetadataCacheService
  {
    private readonly IUserService userService;
    private readonly IBusinessObjectService businessObjectService;

    private volatile IUdfMetadata cachedUdfMetadata = new UdfMetadata(new List<IUdfNamespace>().AsReadOnly());
    private volatile Dictionary<(string Namespace, string FieldName), List<Type>> linkedBusinessObjects = new Dictionary<(string Namespace, string FieldName), List<Type>>();

    public UdfMetadataCacheService(IBroadcastService broadcastService, IUserService userService, IBusinessObjectService businessObjectService)
      : base(broadcastService)
    {
      this.userService = userService;
      this.businessObjectService = businessObjectService;
    }

    protected override void RefreshData()
    {
      IList<UdfMetadataEntity> udfMetadata;

      var systemToken = userService.CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        udfMetadata = dataSession.GetAll<UdfMetadataEntity>(systemToken, false);
        transaction.Commit();
      }

      cachedUdfMetadata = MapUdfMetadata(udfMetadata);
      linkedBusinessObjects = MapBusinessObjectLinkage(udfMetadata);

      RaiseRefreshed();
    }

    protected override ICriteria GetFilterCriteria(string category)
    {
      var criteria = Criteria.For(typeof(UdfMetadataEntity))
        .Add(Expression.Eq(nameof(UdfMetadataEntity.Namespace), category));

      return criteria;
    }

    public IDictionary<(string Namespace, string FieldName), List<Type>> GetLinkedBusinessObjectTypes() =>
      new ReadOnlyDictionary<(string Namespace, string FieldName), List<Type>>(linkedBusinessObjects);

    public IUdfMetadata GetUdfMetadata() => cachedUdfMetadata;

    private Dictionary<(string Namespace, string FieldName), List<Type>> MapBusinessObjectLinkage(IList<UdfMetadataEntity> entities)
    {
      var businessObjects = businessObjectService.GetAll();
      var result = new Dictionary<(string Namespace, string FieldName), List<Type>>(new StringTupleComparer());

      foreach (var entity in entities)
      {
        if (result.TryGetValue((entity.Namespace, entity.FieldName), out var existingList))
        {
          AddItems(existingList, entity.BusinessObjectName);
        }
        else
        {
          var typeList = new List<Type>();
          result.Add((entity.Namespace, entity.FieldName), typeList);
          AddItems(typeList, entity.BusinessObjectName);
        }
      }

      return result;

      void AddItems(List<Type> items, string businessObjectName)
      {
        foreach (var businessObject in businessObjects)
        {
          if (!string.Equals(businessObject.BusinessObject.Name, businessObjectName, StringComparison.OrdinalIgnoreCase))
            continue;

          items.AddRange(businessObject.Types);
        }
      }
    }

    // Performance is a priority here, so avoid extra allocation via LINQ, etc.
    private static IUdfMetadata MapUdfMetadata(IList<UdfMetadataEntity> entities)
    {
      if (entities.Count == 0)
        return new UdfMetadata(new List<IUdfNamespace>().AsReadOnly());
      
      var udfFields = new Dictionary<string, IList<IUdfField>>();

      foreach (var entity in entities.Where(entity => !entity.IsDeletePending))
      {
        var udfField = new UdfField(entity.Id32, entity.BusinessObjectName, entity.FieldName,
          (DataType) entity.DataType, entity.Required, entity.Namespace, entity.Metadata);

        if (udfFields.TryGetValue(entity.Namespace, out var list))
        {
          list.Add(udfField);
        }
        else
        {
          udfFields.Add(entity.Namespace, new List<IUdfField> {udfField});
        }
      }

      IList<IUdfNamespace> namespaces = new List<IUdfNamespace>();

      foreach (var udfField in udfFields)
      {
        namespaces.Add(new UdfNamespace(udfField.Key, new ReadOnlyCollection<IUdfField>(udfField.Value)));
      }
      
      var udfMetadata = new UdfMetadata(new ReadOnlyCollection<IUdfNamespace>(namespaces));
      return udfMetadata;
    }

    private class StringTupleComparer : IEqualityComparer<(string, string)>
    {
      public bool Equals((string, string) x, (string, string) y)
      {
        return string.Equals(x.Item1, y.Item1, StringComparison.OrdinalIgnoreCase)
          && string.Equals(x.Item2, y.Item2, StringComparison.OrdinalIgnoreCase);
      }

      public int GetHashCode((string, string) obj)
      {
        unchecked
        {
          return (obj.Item1.GetHashCode() * 397) ^ obj.Item2.GetHashCode();
        }
      }
    }
  }
}