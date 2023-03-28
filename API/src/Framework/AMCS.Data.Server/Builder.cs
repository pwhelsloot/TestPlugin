using System;

namespace AMCS.Data.Server
{
  public abstract class Builder<T> : IBuilder<T>
  {
    protected readonly T Entity = Activator.CreateInstance<T>();

    public T Build() => Entity;
  }
}