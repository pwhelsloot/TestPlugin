using AMCS.Data.Entity;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  // This is a marker type that should not be used for any purpose other
  // than to drive Swagger metadata generation. This marker type is used by
  // ApiEntityContractProvider to generate custom Newtonsoft.Json metadata
  // from EntityObjectAccessor, to mimic the way that
  // EntityObjectMessageServiceController serializes entities.
  [ApiExplorer(ContractProvider = typeof(ApiEntityContractProvider))]
  internal class ApiEntityDescription<T>
    where T : EntityObject
  {
  }
}
