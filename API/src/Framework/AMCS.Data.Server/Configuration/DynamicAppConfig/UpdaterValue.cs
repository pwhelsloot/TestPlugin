namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;

  public struct UpdaterValue
  {
    private readonly object value;

    public bool HasValue { get; }

    public object Value
    {
      get
      {
        if (!HasValue)
          throw new InvalidOperationException("Unable to get Value, value has not been set!");
        return value;
      }
    }

    public UpdaterValue(object value)
    {
      this.value = value;
      HasValue = true;
    }
  }
}
