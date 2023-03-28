using System.Threading.Tasks;
using AMCS.Data;

namespace AMCS.ApiService.Abstractions
{
  public interface IAsyncMessageService<in TRequest, TResponse>
  {
    Task<TResponse> Perform(ISessionToken userId, TRequest message);
  }
}
