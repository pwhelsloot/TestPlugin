using System;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  /// <summary>
  /// Represents a foreign key relationship to allow inclusion of entities.
  /// </summary>
  public interface IEntityObjectParentRelationship : IEntityObjectRelationship
  {
    /// <summary>
    /// The foreign key property.
    /// </summary>
    EntityObjectProperty Property { get; }

    /// <summary>
    /// The target entity the property links to.
    /// </summary>
    Type Target { get; }
  }
}
