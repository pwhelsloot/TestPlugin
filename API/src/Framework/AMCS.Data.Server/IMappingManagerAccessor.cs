using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration.Mapping.Manager;

namespace AMCS.Data.Server
{
  public interface IMappingManagerAccessor
  {
    /// <summary>
    /// Gets the business object string mapping manager.
    /// </summary>
    /// <value>
    /// The business object string mapping manager.
    /// </value>
    IMappingManager BusinessObjectStringMappingManager { get; }

    /// <summary>
    /// Gets the entity object mapping manager.
    /// </summary>
    /// <value>
    /// The entity object mapping manager.
    /// </value>
    IMappingManager EntityObjectMappingManager { get; }

    /// <summary>
    /// Gets the search result mapping manager.
    /// </summary>
    /// <value>
    /// The search result mapping manager.
    /// </value>
    IMappingManager SearchResultMappingManager { get; }
  }
}
