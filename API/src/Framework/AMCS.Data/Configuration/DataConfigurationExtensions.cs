using System;
using System.Collections.Generic;
using AMCS.Data.Configuration.TimeZones;
using Autofac;

namespace AMCS.Data.Configuration
{
  public static class DataConfigurationExtensions
  {
    public static void SetProjectConfiguration(this DataConfiguration self, IProjectConfiguration configuration)
    {
      if (string.IsNullOrEmpty(configuration.ServiceRootName))
        throw new InvalidOperationException("ServiceRootName must be set on Platform Configuration!");

      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<IProjectConfiguration>();
    }

    public static void SetServiceRootsConfiguration(
      this DataConfiguration self,
      IServiceRootsConfiguration configuration, 
      string serviceRootName)
    {
      self.ContainerBuilder
        .RegisterInstance(new ServiceRootResolver(configuration, serviceRootName))
        .As<IServiceRootResolver>();
    }

    public static void SetConnectionStringsConfiguration(
      this DataConfiguration self,
      Dictionary<string, IConnectionString> connectionStrings)
    {
      self.ContainerBuilder
        .RegisterInstance(new ConnectionStringResolver(connectionStrings))
        .As<IConnectionStringResolver>();
    }

    public static void SetTimeZoneConfiguration(this DataConfiguration self, ITimeZoneConfiguration configuration)
    {
      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<ITimeZoneConfiguration>();
    }

    public static void ConfigureTimeZones(
      this DataConfiguration self,
      IDateTimeZoneProviderProvider dateTimeZoneProviderProvider)
    {
      self.SetTimeZoneConfiguration(new TimeZoneConfiguration(dateTimeZoneProviderProvider));
    }
    
    public static void ConfigureTimeZones(
      this DataConfiguration self,
      IDateTimeZoneProviderProvider dateTimeZoneProviderProvider, 
      INeutralTimeZoneIdProvider neutralTimeZoneIdProvider)
    {
      self.SetTimeZoneConfiguration(new TimeZoneConfiguration(neutralTimeZoneIdProvider, dateTimeZoneProviderProvider));
    }
  }
}
