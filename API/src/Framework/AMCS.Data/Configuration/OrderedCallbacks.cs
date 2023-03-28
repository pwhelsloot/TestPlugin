using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration
{
  public class OrderedCallbacks
  {
    private readonly List<(Action Action, int Order)> callbacks = new List<(Action Action, int Order)>();
    private readonly object syncRoot = new object();
    public void Register(Action action, int order = 0)
    {
      lock (syncRoot)
      {
        callbacks.Add((action, order));
      }
    }
    public void Raise()
    {
      List<(Action Action, int Order)> callbacks;
      lock (syncRoot)
      {
        callbacks = this.callbacks.ToList();
      }
      foreach (var callback in callbacks.OrderBy(p => p.Order))
      {
        callback.Action();
      }
    }
  }
  public class OrderedCallbacks<T>
  {
    private readonly List<(Action<T> Action, int Order)> callbacks = new List<(Action<T> Action, int Order)>();
    private readonly object syncRoot = new object();
    public void Register(Action<T> action, int order = 0)
    {
      lock (syncRoot)
      {
        callbacks.Add((action, order));
      }
    }
    public void Raise(T value)
    {
      List<(Action<T> Action, int Order)> callbacks;
      lock (syncRoot)
      {
        callbacks = this.callbacks.ToList();
      }
      foreach (var callback in callbacks.OrderBy(p => p.Order))
      {
        callback.Action(value);
      }
    }
  }
}
