using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public interface IEntityObjectAsyncMessageService<in TEntity, in TRequest, TResponse>
    where TEntity : EntityObject
  {
    Task<TResponse> Perform(ISessionToken userId, TEntity entity, TRequest request, IDataSession dataSession);
  }
}
