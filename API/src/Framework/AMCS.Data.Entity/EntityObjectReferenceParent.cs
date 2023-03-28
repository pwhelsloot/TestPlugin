namespace AMCS.Data.Entity
{
  /// <summary>
  /// Stores static information about a parent reference on an entity
  /// </summary>
  internal class EntityObjectReferenceParent : IEntityObjectReference
  {
    private readonly EntityObjectProperty property;
    private readonly EntityParentAttribute attribute;

    public EntityObjectAccessor TargetAccessor { get; }

    public string PropertyName => property.Name;
    public string MainColumnName => attribute.ForeignKeyColumn;
    public string TargetColumnName => TargetAccessor.KeyName;
    public bool IsCartesianExplosionRisk => false;

    public EntityObjectReferenceParent(EntityObjectProperty property)
    {
      this.property = property;
      this.attribute = ((EntityObjectReferenceMetadataParent)property.Reference).Attribute;
      this.TargetAccessor = EntityObjectAccessor.ForType(property.Type);
    }

    public void Reset(EntityObject mainObject) { }

    public void Assign(EntityObject mainObject, EntityObject target)
    {
      this.property.SetValue(mainObject, target);
    }
  }
}
