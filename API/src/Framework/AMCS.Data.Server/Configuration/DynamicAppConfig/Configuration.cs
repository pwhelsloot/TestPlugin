namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Data;
  using Data.Support;
  using Providers;
  using Providers.DefaultValue;

  public abstract class Configuration
  {
    internal abstract string ConfigId { get; }
    internal abstract ConfigurationVisibility ConfigurationVisibility { get; }
    internal abstract IConfigurationProvider[] Providers { get; }
    internal abstract object GetValueUntyped(ISessionToken userId);
    internal abstract void RecalculateValue();

    internal virtual T ConvertValue<T>(object untypedValue)
    {
      try
      {
        return UntypedStringConversionUtils.ParseUntypedString<T>(untypedValue);
      }
      catch (Exception exception)
      {
        throw new InvalidOperationException($"Configuration value mismatch: Cannot convert {untypedValue.GetType()} to {typeof(T)} on configuration '{ConfigId}'", exception);
      }
    }
  }

  public class Configuration<T> : Configuration
  {
    private readonly IValueCombiner<T> valueCombiner;
    private readonly IList<Updater> updaters = new List<Updater>();
    private readonly Updater defaultUpdater;

    private volatile object value;

    public T Value => (T)value;
    internal override string ConfigId { get; }
    internal override ConfigurationVisibility ConfigurationVisibility { get; }
    internal override IConfigurationProvider[] Providers { get; }

    internal Configuration(
      string configId,
      IValueCombiner<T> valueCombiner,
      ConfigurationVisibility configurationVisibility,
      IConfigurationProvider[] providers,
      IConfigurationSourceManager configurationSources)
    {
      this.ConfigId = configId;
      this.ConfigurationVisibility = configurationVisibility;
      Providers = providers;
      this.valueCombiner = valueCombiner;

      var providersSorted = providers
        .Where(provider => configurationSources.GetOrder(provider) != null)
        .OrderByDescending(configurationSources.GetOrder);


      foreach (var provider in providersSorted)
      {
        var updater = new Updater(this);
        if (provider is DefaultValueProvider)
        {
          if (defaultUpdater != null)
          {
            throw new InvalidOperationException(
              $"More than one default provider provided for configuration '{configId}'");
          }
          defaultUpdater = updater;
        }
        else
        {
          updaters.Add(updater);
        }
        configurationSources.Register<T>(provider, updater);
      }
    }

    internal override object GetValueUntyped(ISessionToken userId) => Value;

    internal override void RecalculateValue()
    {
      value = CalculateValue();
    }

    private T CalculateValue()
    {
      var values = new List<T>();
      foreach (var updater in updaters)
      {
        var updaterValue = updater.Value;
        if (updaterValue.HasValue)
        {
          values.Add(ConvertValue<T>(updaterValue.Value));
        }
      }

      if (values.Any())
        return valueCombiner.Combine(values);

      if (defaultUpdater != null)
        return ConvertValue<T>(defaultUpdater.Value.Value);

      throw new InvalidOperationException($"No value provided for configuration '{ConfigId}'");
    }
  }
}
