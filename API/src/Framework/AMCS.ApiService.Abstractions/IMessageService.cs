using AMCS.Data;

namespace AMCS.ApiService.Abstractions
{
  public interface IMessageService<in TRequest, out TResponse>
  {
    TResponse Perform(ISessionToken userId, TRequest message);
  }
}
