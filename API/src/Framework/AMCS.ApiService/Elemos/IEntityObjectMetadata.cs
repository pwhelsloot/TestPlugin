using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos
{
  public interface IEntityObjectMetadata
  {
    Type EntityType { get; }

    string ObjectName { get; }

    string CollectionName { get; }

    IList<IEntityObjectRelationship> Relationships { get; }

    IList<IOperation> Operations { get; }
  }
}
