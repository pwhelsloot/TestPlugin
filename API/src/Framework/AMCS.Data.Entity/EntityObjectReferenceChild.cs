using System;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.Entity
{
  /// <summary>
  /// Stores static information about a child reference on an entity.
  /// </summary>
  internal class EntityObjectReferenceChild : IEntityObjectReference
  {
    private readonly EntityObjectProperty property;
    private readonly EntityObjectAccessor mainAccessor;
    private readonly EntityObjectProperty reverseProperty;
    private readonly EntityChildAttribute attribute;
    private ICollectionManager collectionManager;

    public EntityObjectAccessor TargetAccessor { get; }

    public string PropertyName => property.Name;
    public string MainColumnName => mainAccessor.KeyName;
    public string TargetColumnName => attribute.ForeignKeyColumn;
    public bool IsCartesianExplosionRisk => !attribute.Sparse;

    public EntityObjectReferenceChild(EntityObjectAccessor mainAccessor, EntityObjectProperty property, EntityObjectAccessor targetAccessor)
    {
      this.property = property;
      this.mainAccessor = mainAccessor;
      this.attribute = ((EntityObjectReferenceMetadataChild)property.Reference).Attribute;
      this.TargetAccessor = targetAccessor;

      this.reverseProperty = TargetAccessor.Properties.SingleOrDefault(
        prop => prop.Reference != null
        && prop.Type == property.EntityType
        && prop.Reference.IsParentForChild(attribute));
      this.collectionManager = GetCollectionManager(property.Type);
    }

    public void Reset(EntityObject mainObject)
    {
      var collection = property.GetValue(mainObject);
      if (collection != null)
        collectionManager.Clear(collection);
      else
        property.SetValue(mainObject, collectionManager.Create());
    }

    /// <summary>
    /// Adds the target entity to the main entity's collection
    /// Also add the main object to the target entity's parent reference (if it has one).
    /// </summary>
    public void Assign(EntityObject mainObject, EntityObject target)
    {
      var collection = property.GetValue(mainObject);
      if (collection != null)
        collectionManager.Add(collection, target);

      // Assign this entity as the parent of the value entity
      if (reverseProperty != null)
        reverseProperty.SetValue(target, mainObject);
    }

    private static ICollectionManager GetCollectionManager(Type type)
    {
      var genericType = GetContainerManagerType(type);
      Type specializedType = genericType.MakeGenericType(type.GenericTypeArguments);
      return (ICollectionManager)Activator.CreateInstance(specializedType);
    }

    private static Type GetContainerManagerType(Type type)
    {
      var genericType = type.GetGenericTypeDefinition();
      if (genericType == typeof(IList<>) || genericType == typeof(List<>))
        return typeof(ListManager<>);
      if (genericType == typeof(ISet<>) || genericType == typeof(HashSet<>))
        return typeof(SetManager<>);
      throw new ArgumentException($"Unsupported child reference type '{type.Name}'");
    }

    private interface ICollectionManager
    {
      object Create();
      void Clear(object collection);
      void Add(object collection, EntityObject value);
    }

    private class ListManager<T> : ICollectionManager
      where T : EntityObject
    {
      public object Create()
      {
        return new List<T>();
      }

      public void Clear(object collection)
      {
        ((ICollection<T>)collection).Clear();
      }

      public void Add(object collection, EntityObject value)
      {
        ((ICollection<T>)collection).Add((T)value);
      }
    }

    private class SetManager<T> : ICollectionManager
        where T : EntityObject
    {
      public object Create()
      {
        return new HashSet<T>();
      }

      public void Clear(object collection)
      {
        ((ICollection<T>)collection).Clear();
      }

      public void Add(object collection, EntityObject value)
      {
        ((ICollection<T>)collection).Add((T)value);
      }
    }
  }
}
