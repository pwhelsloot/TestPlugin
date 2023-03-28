using System;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  /// <summary>
  /// Represents the reverse of a foreign key relationship to allow expansion
  /// of related entities.
  /// </summary>
  public interface IEntityObjectChildRelationship : IEntityObjectRelationship
  {
    /// <summary>
    /// The entity type we want to expand.
    /// </summary>
    Type Target { get; }

    /// <summary>
    /// The property in the target entity that links to our primary key.
    /// </summary>
    EntityObjectProperty TargetProperty { get; }
  }
}
