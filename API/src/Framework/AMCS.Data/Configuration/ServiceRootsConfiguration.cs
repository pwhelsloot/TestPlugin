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

namespace AMCS.Data.Configuration
{
  public interface IServiceRootsConfiguration
  {
    IEnumerable<IServiceRootConfiguration> GetServiceRoots();
  }

#if NETFRAMEWORK 

  public class ServiceRootsConfigurationSection : ConfigurationSection, IServiceRootsConfiguration
  {
    [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
    public ServiceRootElementCollection ServiceRoots
    {
      get => (ServiceRootElementCollection)this[string.Empty];
      set => this[string.Empty] = value;
    }

    IEnumerable<IServiceRootConfiguration> IServiceRootsConfiguration.GetServiceRoots()
    {
      return ServiceRoots?.Cast<ServiceRootElement>() ?? Enumerable.Empty<ServiceRootElement>();
    }

  }

#else

  public class ServiceRootsConfigurationSection : IServiceRootsConfiguration
  { 
    public IList<ServiceRootElement> ServiceRoots { get; }

    IEnumerable<IServiceRootConfiguration> IServiceRootsConfiguration.GetServiceRoots()
    {
      return ServiceRoots;
    }

    public ServiceRootsConfigurationSection (XElement element)
    {
      ServiceRoots = element != null
        ? new ReadOnlyCollection<ServiceRootElement>(element.Elements("add").Select(x => new ServiceRootElement(x)).ToList())
        : (IList<ServiceRootElement>)new ServiceRootElement[0];
    }
  }

#endif

#if NETFRAMEWORK

  public class ServiceRootElementCollection : ConfigurationElementCollection
  {
    protected override ConfigurationElement CreateNewElement()
    {
      return new ServiceRootElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((ServiceRootElement)element).Name;
    }
  }
    
# endif

  public interface IServiceRootConfiguration
  {
    string Name { get; }

    string Url { get; }

  }

#if NETFRAMEWORK 

  public class ServiceRootElement : ConfigurationElement, IServiceRootConfiguration
  {
    [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
    public string Name
    {
      get => (string)this["name"];
      set => this["name"] = value;
    }

    [ConfigurationProperty("url", IsRequired = true)]
    public string Url
    {
      get => (string)this["url"];
      set => this["url"] = value;
    }

  }

#else

  public class ServiceRootElement : IServiceRootConfiguration
  { 
    public string Name { get; }

    public string Url { get; }

    public ServiceRootElement (XElement element)
    {
      Name = element?.Attribute("name")?.Value != null ? element.Attribute("name").Value : default(string);
      Url = element?.Attribute("url")?.Value != null ? element.Attribute("url").Value : default(string);
    }
  }

#endif

}
