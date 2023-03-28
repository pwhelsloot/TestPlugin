//-----------------------------------------------------------------------------
// <copyright file="EntityObjectMapping.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class EntityObjectMapping : IEntityObjectMapping
  {
    public string Id { get; set; }

    public Guid FileId { get; set; }

    public string Table { get; set; }

    public string AuditTable { get; set; }

    public string AuditQuery { get; set; }

    public string AuditKeyField { get; set; }

    public IList<IEntityObjectProperty> Properties { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool MappedStructureEquals(IMapping other)
    {
      EntityObjectMapping that = (EntityObjectMapping)other;
      if (!this.Id.Equals(that.Id))
      {
        return false;
      }

      int thisPropertyCount = 0;
      int thatPropertyCount = 0;
      if (this.Properties != null)
      {
        thisPropertyCount = this.Properties.Count;
      }

      if (that.Properties != null)
      {
        thatPropertyCount = that.Properties.Count;
      }

      if (thisPropertyCount != thatPropertyCount)
      {
        return false;
      }

      // Still need to check if the properties have changed.
      foreach (IEntityObjectProperty thisProperty in this.Properties)
      {
        // We have the same number of properties, if there are to be any differences then at 
        // least one will have to have a different name
        if (that.Properties.FirstOrDefault(p => p.PropertyName.Equals(thisProperty.PropertyName)) == null)
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// See comments in IMapping.
    /// 
    /// This method ignores all fields related to ORM, it is assumed that "this" mapping is correct here.
    /// </summary>
    /// <param name="mergeIn"></param>
    public void MergeWith(IMapping mergeIn)
    {
      if (!(mergeIn is EntityObjectMapping))
      {
        throw new Exception("Cannot merge type of " + this.GetType().FullName + " and " + mergeIn.GetType().FullName);
      }

      EntityObjectMapping that = (EntityObjectMapping)mergeIn;

      if (!this.Id.Equals(that.Id))
      {
        throw new Exception("The owner of 'mergeIn.Owner' is different to 'this.Owner', (" + that.Id + " versus " + this.Id + ")cannot continue.");
      }

      // Merge audit settings
      if (!string.IsNullOrWhiteSpace(that.AuditKeyField))
      {
        if (this.AuditKeyField != that.AuditKeyField)
        {
          this.AuditKeyField = that.AuditKeyField;
        }
      }

      if (!string.IsNullOrWhiteSpace(that.AuditTable))
      {
        if (this.AuditTable != that.AuditTable)
        {
          this.AuditTable = that.AuditTable;
        }
      }

      if (!string.IsNullOrWhiteSpace(that.AuditQuery))
      {
        if (this.AuditQuery != that.AuditQuery)
        {
          this.AuditQuery = that.AuditQuery;
        }
      }

      foreach (IEntityObjectProperty thatProperty in that.Properties)
      {
        IEntityObjectProperty thisProperty = this.Properties.FirstOrDefault(p => p.PropertyName.Equals(thatProperty.PropertyName));

        // Is this a new field in "mergeIn"?
        if (thisProperty == null)
        {
          this.Properties.Add(thatProperty);
        }
        else
        {
          thisProperty.Width = thatProperty.Width;

          if (thatProperty.StringFormat != null)
          {
            thisProperty.StringFormat = thatProperty.StringFormat;
          }

          if (thisProperty.FailValidationTest != thatProperty.FailValidationTest)
          {
            thisProperty.FailValidationTest = thatProperty.FailValidationTest;
          }

          if (thatProperty.String != null)
          {
            if (thisProperty.String == null)
            {
              thisProperty.String = new EntityObjectTranslatableLocaleString();
            }

            if (!thatProperty.String.Value.Equals(thisProperty.String.Value))
            {
              thisProperty.String.Value = thatProperty.String.Value;
              thisProperty.String.Locale = thatProperty.String.Locale;
            }

            IEntityObjectLocaleString thatErrorString = (IEntityObjectLocaleString)thatProperty.String;
            IEntityObjectLocaleString thisErrorString = (IEntityObjectLocaleString)thisProperty.String;
            if (thatErrorString.ErrorText != thisErrorString.ErrorText)
            {
              thisErrorString.ErrorText = thatErrorString.ErrorText;
            }

            if (thisProperty.String.Translations == null)
            {
              thisProperty.String.Translations = new List<ILocaleString>();
            }

            if (thatProperty.String.Translations != null && thatProperty.String.Translations.Count > 0)
            {
              foreach (IEntityObjectLocaleString thatTranslation in thatProperty.String.Translations)
              {
                IEntityObjectLocaleString thisTranslation = (IEntityObjectLocaleString)thisProperty.String.Translations.FirstOrDefault(t => t.Locale.Equals(thatTranslation.Locale));

                // This translation doesn't exist in "this", add it.
                if (thisTranslation == null)
                {
                  thisProperty.String.Translations.Add(thatTranslation);
                }
                else
                {
                  // Otherwise see if there are any changes
                  if (!thatTranslation.Value.Equals(thisTranslation.Value))
                  {
                    thisTranslation.Value = thatTranslation.Value;
                  }
                    
                  if (thatTranslation.ErrorText == null || !thatTranslation.ErrorText.Equals(thisTranslation.ErrorText))
                  {
                    thisTranslation.ErrorText = thatTranslation.ErrorText;
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}