//-----------------------------------------------------------------------------
// <copyright file="EntityObjectTranslator.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.Translate
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration.Mapping.Manager;

  /// <summary>
  /// Used to translate the property names of EntityObjects
  /// </summary>
  public class EntityObjectTranslator : StringCollectionTranslator
  {
    private IMappingManager mappingManager;
    private IEntityObjectMapping mapping;
    private IList<string> displayablePropertyOrder = new List<string>();

    public EntityObjectTranslator(string entityObjectTypeFullName, IMappingManager entityObjectMappingManager)
      : base(ResourceStringOwnerType.Entity, entityObjectTypeFullName)
    {
      this.mappingManager = entityObjectMappingManager;

      if (this.mappingManager.IsMapped(entityObjectTypeFullName))
      {
        this.mapping = (IEntityObjectMapping)this.mappingManager.GetMapping(entityObjectTypeFullName);

        foreach (IEntityObjectProperty property in this.mapping.Properties)
        {
          if (property.Width.GetValueOrDefault(0) != 0)
          {
            this.displayablePropertyOrder.Add(property.PropertyName);
          }
        }
      }
    }

    public string AuditTable
    {
      get { return this.mapping == null ? string.Empty : this.mapping.AuditTable; }
    }

    public string AuditQuery
    {
      get { return this.mapping == null ? string.Empty : this.mapping.AuditQuery; }
    }

    public string AuditKeyField
    {
      get { return this.mapping == null ? string.Empty : this.mapping.AuditKeyField; }
    }

    public Guid FileIdField
    {
      get { return this.mapping == null ? Guid.Empty : this.mapping.FileId; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName">The name of the property you want to get the translated description of</param>
    /// <returns>A translated version of the property name, or the property name if a translation doesn't exist</returns>
    public string GetPropertyName(string propertyName)
    {
      if (this.mapping == null)
      {
        return null;
      }

      IEntityObjectProperty prop = this.mapping.Properties.FirstOrDefault(p => p.PropertyName.Equals(propertyName));
      if (prop != null && prop.String != null)
      {
        if (CultureInfo.CurrentUICulture.Name.Equals(prop.String.Locale))
        {
          return prop.String.Value;
        }

        ILocaleString translation = null;
        if (prop.String.Translations != null && prop.String.Translations.Count > 0)
        {
          translation = prop.String.Translations.FirstOrDefault(t => t.Locale.Equals(CultureInfo.CurrentUICulture.Name));
        }

        if (translation == null || string.IsNullOrWhiteSpace(translation.Value))
        {
          translation = this.GetFallbackString(CultureInfo.CurrentUICulture.Name, prop.String);
        }

        // Should always succeed because of GetFallbackString(...)
        if (translation != null && !string.IsNullOrWhiteSpace(translation.Value))
        {
          return translation.Value;
        }
      }

      return propertyName;
    }

    /// <summary>
    /// This obviously isn't anything to do with translation but having this here keeps things simple.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public int GetPropertyDisplayWidth(string propertyName)
    {
      if (this.mapping != null)
      {
        IEntityObjectProperty prop = this.mapping.Properties.FirstOrDefault(p => p.PropertyName.Equals(propertyName));
        if (prop != null && prop.String != null)
        {
          return prop.Width.GetValueOrDefault(0);
        }
      }

      return 0;
    }

    /// <summary>
    /// This obviously isn't anything to do with translation but having this here keeps things simple.
    /// </summary>
    /// <returns></returns>
    public int GetDisplayablePropertyCount()
    {
      return this.displayablePropertyOrder.Count;
    }

    /// <summary>
    /// This obviously isn't anything to do with translation but having this here keeps things simple.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public int GetPropertyDisplayIndex(string propertyName)
    {
      return this.displayablePropertyOrder.IndexOf(propertyName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns>StringFormat string if exists, otherwise null</returns>
    public string GetPropertyStringFormat(string propertyName)
    {
      if (this.mapping != null)
      {
        IEntityObjectProperty prop = this.mapping.Properties.FirstOrDefault(p => p.PropertyName.Equals(propertyName));
        if (prop != null && prop.String != null)
        {
          return prop.StringFormat;
        }
      }

      return null;
    }
  }
}
