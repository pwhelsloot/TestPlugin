using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos
{
  /// <summary>
  /// Models foreign key and reversed foreign key relationships.
  /// </summary>
  public interface IEntityObjectRelationship
  {
    /// <summary>
    /// The name of the relationship to allow it to be identified through the API.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The kind of the relationship.
    /// </summary>
    EntityObjectRelationshipKind Kind { get; }
  }
}
