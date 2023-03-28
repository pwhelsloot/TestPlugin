namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using Data;
  using Providers;
  using Providers.DefaultValue;

  public class TenantConfiguration<T> : TenantConfigurationBase
  {
    private static readonly object EmptyValue = new object();

    private readonly object syncRoot = new object();
    private readonly IValueCombiner<T> valueCombiner;
    private readonly IList<TenantConfigurationValueUpdater> updaters = new List<TenantConfigurationValueUpdater>();
    private readonly TenantConfigurationValueUpdater defaultUpdater;

    private volatile Dictionary<string, T> valuePerTenant = new Dictionary<string, T>();
    private volatile object globalValue = EmptyValue;
    public UpdaterValue GlobalValue => globalValue == EmptyValue ? new UpdaterValue() : new UpdaterValue(globalValue);

    internal override string ConfigId { get; }
    internal override ConfigurationVisibility ConfigurationVisibility { get; }
    internal override IConfigurationProvider[] Providers { get; }

    internal TenantConfiguration(
      string configId,
      IValueCombiner<T> valueCombiner,
      ConfigurationVisibility configurationVisibility,
      IConfigurationProvider[] providers,
      IConfigurationSourceManager configurationSourceManager)
    {
      this.ConfigId = configId;
      this.ConfigurationVisibility = configurationVisibility;
      this.Providers = providers;
      this.valueCombiner = valueCombiner;
      var providersOrdered = GetOrderedProviders(providers, configurationSourceManager);

      foreach (var provider in providersOrdered)
      {
        var updater = new TenantConfigurationValueUpdater(this);
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
        configurationSourceManager.RegisterTenant<T>(provider, updater);
      }
    }

    /// <summary>
    /// Orders the Providers in a Descending Order
    /// </summary>
    /// <param name="providers">Providers to sort</param>
    /// <param name="configurationSourceManager">Configuration Sources</param>
    /// <returns></returns>
    private static IOrderedEnumerable<IConfigurationProvider> GetOrderedProviders(IConfigurationProvider[] providers, IConfigurationSourceManager configurationSourceManager)
    {
      return providers
        .Where(provider => configurationSourceManager.GetOrder(provider) != null)
        .OrderByDescending(configurationSourceManager.GetOrder);
    }

    public T GetValue(ISessionToken userId)
    {
      if (valuePerTenant != null && valuePerTenant.TryGetValue(userId.TenantId, out T value))
        return value;

      var globalValue = GlobalValue;
      if (globalValue.HasValue)
        return (T)globalValue.Value;

      throw new InvalidOperationException(
        $"No value provided for configuration '{ConfigId}' and tenant '{userId.TenantId}'");
    }

    internal override object GetValueUntyped(ISessionToken userId) => GetValue(userId);

    internal override void RecalculateValue()
    {
      lock (syncRoot)
      {
        RecalculateTenantValues();

        globalValue = RecalculateGlobalValue();
      }
    }

    private void RecalculateTenantValues()
    {
      var clone = new Dictionary<string, T>(valuePerTenant);

      IList<string> tenantIds = valuePerTenant.Keys.ToList();
      foreach (string tenantId in tenantIds)
      {
        RecalculateValues(clone, tenantId);
      }

      valuePerTenant = clone;
    }

    private object RecalculateGlobalValue()
    {
      var values = new List<T>();
      foreach (var updater in updaters)
      {
        var updaterValue = updater.GlobalValue;
        if (updaterValue.HasValue)
        {
          values.Add(ConvertValue<T>(updaterValue.Value));
        }
      }

      if (values.Any())
        return valueCombiner.Combine(values);

      if (defaultUpdater != null)
        return ConvertValue<T>(defaultUpdater.GlobalValue.Value);

      return EmptyValue;
    }

    internal override void RecalculateTenantValue(string tenantId)
    {
      lock (syncRoot)
      {
        var clone = new Dictionary<string, T>(valuePerTenant);
        RecalculateValues(clone, tenantId);
        valuePerTenant = clone;
      }
    }

    private void RecalculateValues(Dictionary<string, T> localValuePerTenant, string tenantId)
    {
      var values = new List<T>();
      foreach (var updater in updaters)
      {
        var updaterValue = updater.GetValue(tenantId);
        if (updaterValue.HasValue)
        {
          values.Add(ConvertValue<T>(updaterValue.Value));
        }
      }

      if (values.Any())
      {
        localValuePerTenant[tenantId] = valueCombiner.Combine(values);
      }
      else
      {
        localValuePerTenant.Remove(tenantId);
      }
    }

    private class TenantConfigurationValueUpdater : ITenantConfigurationValueUpdater
    {
      private readonly TenantConfigurationBase configuration;
      private readonly ConcurrentDictionary<string, object> valuePerTenant = new ConcurrentDictionary<string, object>();
      private volatile object globalValue = EmptyValue;

      public UpdaterValue GlobalValue => CreateUpdaterValue(globalValue);

      public TenantConfigurationValueUpdater(TenantConfigurationBase configuration)
      {
        this.configuration = configuration;
      }

      public UpdaterValue GetValue(string tenantId)
      {
        if (valuePerTenant.TryGetValue(tenantId, out object value))
          return CreateUpdaterValue(value);
        return GlobalValue;
      }

      public void SetValue(string tenantId, object value)
      {
        valuePerTenant[tenantId] = value;
        configuration.RecalculateTenantValue(tenantId);
      }

      public void ClearValue(string tenantId)
      {
        valuePerTenant.TryRemove(tenantId, out _);
        configuration.RecalculateTenantValue(tenantId);
      }

      public void SetValue(object value) => SetValueInner(value);
      public void ClearValue() => SetValueInner(EmptyValue);

      private void SetValueInner(object value)
      {
        globalValue = value;
        configuration.RecalculateValue();
      }

      private UpdaterValue CreateUpdaterValue(object value)
      {
        return value == EmptyValue ? new UpdaterValue() : new UpdaterValue(value);
      }
    }
  }
}
