using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos
{
  internal interface IEntityObjectMetadataManager
  {
    IEntityObjectMetadata ForType(Type entityType);

    IEntityObjectMetadata ForCollection(string collectionName);

    IOperation FindOperation(string operationName);
  }
}
