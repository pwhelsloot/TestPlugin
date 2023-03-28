namespace AMCS.Data.Server.Services
{
  using System;

  public class TenantsChangedEventArgs : EventArgs
  {
    public TenantsChangedEventArgs()
    {
    }
  }

  public delegate void TenantsChangedEventHandler(TenantsChangedEventArgs e);
}