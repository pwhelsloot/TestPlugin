namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  public class Updater : IConfigurationValueUpdater
  {
    private static readonly object EmptyValue = new object();

    private readonly Configuration configuration;
    private volatile object value = EmptyValue;

    public UpdaterValue Value => value == EmptyValue ? new UpdaterValue() : new UpdaterValue(value);

    public Updater(Configuration configuration)
    {
      this.configuration = configuration;
    }

    public void SetValue(object value) => SetInnerValue(value);
    public void ClearValue() => SetInnerValue(EmptyValue);

    private void SetInnerValue(object value)
    {
      this.value = value;
      configuration.RecalculateValue();
    }
  }
}
