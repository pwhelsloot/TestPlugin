using System.Collections.Generic;

namespace AMCS.Data.Server.Api
{
  public interface IRestApiService
  {
    ApiResult<IList<T>> GetCollection<T>(RestApiService.GetCollectionParams @params);
  }
}