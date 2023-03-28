﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AMCS.TestPlugin.Server.Configuration
{
  public interface ITestPluginConfiguration
  {
    string NeutralTimeZoneId { get; }

    string TenantId { get; }

    ICommsServerConfiguration CommsServer { get; }
  }

#if NETFRAMEWORK 

  public class TestPluginConfigurationSection : ConfigurationSection, ITestPluginConfiguration
  {
    [ConfigurationProperty("neutralTimeZoneId")]
    public string NeutralTimeZoneId
    {
      get => (string)this["neutralTimeZoneId"];
      set => this["neutralTimeZoneId"] = value;
    }

    [ConfigurationProperty("tenantId", IsRequired = true)]
    public string TenantId
    {
      get => (string)this["tenantId"];
      set => this["tenantId"] = value;
    }

    [ConfigurationProperty("commsServer")]
    public CommsServerElement CommsServer
    {
      get => (CommsServerElement)this["commsServer"];
      set => this["commsServer"] = value;
    }

    ICommsServerConfiguration ITestPluginConfiguration.CommsServer => CommsServer;

  }

#else

  public class TestPluginConfigurationSection : ITestPluginConfiguration
  { 
    public string NeutralTimeZoneId { get; }

    public string TenantId { get; }

    public ICommsServerConfiguration CommsServer { get; }

    public TestPluginConfigurationSection (XElement element)
    {
      NeutralTimeZoneId = element?.Attribute("neutralTimeZoneId")?.Value != null ? element.Attribute("neutralTimeZoneId").Value : default(string);
      TenantId = element?.Attribute("tenantId")?.Value != null ? element.Attribute("tenantId").Value : default(string);
      CommsServer = element != null ? new CommsServerElement(element.Element("commsServer")) : null;
    }
  }

#endif

  public interface ICommsServerConfiguration
  {
    ICommsServerServerConfiguration Server { get; }
  }

#if NETFRAMEWORK 

  public class CommsServerElement : ConfigurationElement, ICommsServerConfiguration
  {
    [ConfigurationProperty("server")]
    public CommsServerServerElement Server
    {
      get => (CommsServerServerElement)this["server"];
      set => this["server"] = value;
    }

    ICommsServerServerConfiguration ICommsServerConfiguration.Server => Server;

  }

#else

  public class CommsServerElement : ICommsServerConfiguration
  { 
    public ICommsServerServerConfiguration Server { get; }

    public CommsServerElement (XElement element)
    {
      Server = element != null ? new CommsServerServerElement(element.Element("server")) : null;
    }
  }

#endif

  public interface ICommsServerServerConfiguration
  {
    string Url { get; }

    string AuthKey { get; }

    string Protocol { get; }

    bool AllowAlternateTransport { get; }

    string AzureServiceBusConnectionStringName { get; }

  }

#if NETFRAMEWORK 

  public class CommsServerServerElement : ConfigurationElement, ICommsServerServerConfiguration
  {
    [ConfigurationProperty("url")]
    public string Url
    {
      get => (string)this["url"];
      set => this["url"] = value;
    }

    [ConfigurationProperty("authKey")]
    public string AuthKey
    {
      get => (string)this["authKey"];
      set => this["authKey"] = value;
    }

    [ConfigurationProperty("protocol")]
    public string Protocol
    {
      get => (string)this["protocol"];
      set => this["protocol"] = value;
    }

    [ConfigurationProperty("allowAlternateTransport")]
    public bool AllowAlternateTransport
    {
      get => (bool)this["allowAlternateTransport"];
      set => this["allowAlternateTransport"] = value;
    }

    [ConfigurationProperty("azureServiceBusConnectionStringName")]
    public string AzureServiceBusConnectionStringName
    {
      get => (string)this["azureServiceBusConnectionStringName"];
      set => this["azureServiceBusConnectionStringName"] = value;
    }

  }

#else

  public class CommsServerServerElement : ICommsServerServerConfiguration
  { 
    public string Url { get; }

    public string AuthKey { get; }

    public string Protocol { get; }

    public bool AllowAlternateTransport { get; }

    public string AzureServiceBusConnectionStringName { get; }

    public CommsServerServerElement (XElement element)
    {
      Url = element?.Attribute("url")?.Value != null ? element.Attribute("url").Value : default(string);
      AuthKey = element?.Attribute("authKey")?.Value != null ? element.Attribute("authKey").Value : default(string);
      Protocol = element?.Attribute("protocol")?.Value != null ? element.Attribute("protocol").Value : default(string);
      AllowAlternateTransport = element?.Attribute("allowAlternateTransport")?.Value != null ? bool.Parse(element.Attribute("allowAlternateTransport").Value) : default(bool);
      AzureServiceBusConnectionStringName = element?.Attribute("azureServiceBusConnectionStringName")?.Value != null ? element.Attribute("azureServiceBusConnectionStringName").Value : default(string);
    }
  }

#endif

}

