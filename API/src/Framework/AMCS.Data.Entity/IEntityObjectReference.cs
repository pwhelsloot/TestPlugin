namespace AMCS.Data.Entity
{
  /// <summary>
  /// Defines static information about a reference on an entity
  /// </summary>
  internal interface IEntityObjectReference
  {
    EntityObjectAccessor TargetAccessor { get; }
    string PropertyName { get; }
    string MainColumnName { get; }
    string TargetColumnName { get; }
    bool IsCartesianExplosionRisk { get; }

    void Assign(EntityObject entityA, EntityObject entityB);
    void Reset(EntityObject mainObject);
  }
}
