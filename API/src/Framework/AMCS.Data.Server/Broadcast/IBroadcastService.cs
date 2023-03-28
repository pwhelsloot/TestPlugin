using System;

namespace AMCS.Data.Server.Broadcast
{
  public interface IBroadcastService : IDisposable
  {
    void Broadcast(object obj);
    void On<T>(Action<T> action);
  }
}
