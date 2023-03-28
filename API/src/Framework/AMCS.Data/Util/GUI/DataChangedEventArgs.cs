namespace AMCS.Data.Util.GUI
{
  using System;
  using System.Reflection;

  public class DataChangedEventArgs : EventArgs
  {
    public object PropertyOwner { get; set; }
    public PropertyInfo PropertyChanged { get; set; }
    public bool HasOwnerStateChanged { get; set; }

    public static new DataChangedEventArgs Empty
    {
      get
      {
        return new DataChangedEventArgs();
      }
    }

    public DataChangedEventArgs() { }
  }
}
