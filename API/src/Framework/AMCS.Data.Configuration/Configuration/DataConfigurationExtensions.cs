//-----------------------------------------------------------------------------
// <copyright file="DataConfigurationExtensions.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Configuration
{
  using Autofac;
  using Resource;

  public static class DataConfigurationExtensions
  {
    public static void SetLocalizationConfiguration(this DataConfiguration self, ILanguageResources languageResources)
    {
      self.ContainerBuilder
        .Register(p => new LocalisedStringResourceCache(languageResources))
        .SingleInstance()
        .AutoActivate()
        .As<ILocalisedStringResourceCache>();
    }
  }
}
