//-----------------------------------------------------------------------------
// <copyright file="Finalizer.cs" company="AMCS Group">
//   Copyright © 2019 AMCS Group. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------
namespace AMCS.Data
{
  using System;

  public class Finalizer : IDisposable
  {
    private bool disposed;
    private Action action;

    public Finalizer(Action action)
    {
      this.action = action;
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (action != null)
        {
          action();
          action = null;
        }

        disposed = true;
      }
    }
  }
}

