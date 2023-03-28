//-----------------------------------------------------------------------------
// <copyright file="SearchResultMapping.cs" company="AMCS Group">
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

  public class SearchResultMapping : ISearchResultMapping
  {
    public string Id { get; set; }

    public Guid FileId { get; set; }

    public string AuditTable { get; set; }

    public string AuditQuery { get; set; }

    public string AuditKeyField { get; set; }

    public IList<ISearchResultColumn> Columns { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool MappedStructureEquals(IMapping other)
    {
      SearchResultMapping that = (SearchResultMapping)other;
      if (!this.Id.Equals(that.Id))
      {
        return false;
      }

      int thisColumnCount = 0;
      int thatColumnCount = 0;
      if (this.Columns != null)
      {
        thisColumnCount = this.Columns.Count;
      }

      if (that.Columns != null)
      {
        thatColumnCount = that.Columns.Count;
      }

      if (thisColumnCount != thatColumnCount)
      {
        return false;
      }

      foreach (IColumn thisColumn in this.Columns)
      {
        if (that.Columns.FirstOrDefault(c => c.ColumnName.Equals(thisColumn.ColumnName)) == null)
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// See comments in IMapping.
    /// </summary>
    /// <param name="mergeIn"></param>
    public void MergeWith(IMapping mergeIn)
    {
      if (!(mergeIn is SearchResultMapping))
      {
        throw new Exception("Cannot merge type of " + this.GetType().FullName + " and " + mergeIn.GetType().FullName);
      }

      SearchResultMapping that = (SearchResultMapping)mergeIn;

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

      foreach (ISearchResultColumn thatColumn in that.Columns)
      {
        ISearchResultColumn thisColumn = this.Columns.FirstOrDefault(c => c.ColumnName.Equals(thatColumn.ColumnName));

        // Is this a new field in "mergeIn"?
        if (thisColumn == null)
        {
          this.Columns.Add(thatColumn);
        }
        else
        {
          thisColumn.Width = thatColumn.Width;

          if (thatColumn.StringFormat != null)
          {
            thisColumn.StringFormat = thatColumn.StringFormat;
          }

          if (thatColumn.String != null)
          {
            if (thisColumn.String == null)
            {
              thisColumn.String = new TranslatableLocaleString();
            }
              
            if (!thatColumn.String.Value.Equals(thisColumn.String.Value))
            {
              thisColumn.String.Value = thatColumn.String.Value;
              thisColumn.String.Locale = thatColumn.String.Locale;
            }

            if (thisColumn.String.Translations == null)
            {
              thisColumn.String.Translations = new List<ILocaleString>();
            }

            if (thatColumn.String.Translations != null && thatColumn.String.Translations.Count > 0)
            {
              foreach (ILocaleString thatTranslation in thatColumn.String.Translations)
              {
                ILocaleString thisTranslation = thisColumn.String.Translations.FirstOrDefault(t => t.Locale.Equals(thatTranslation.Locale));

                // This translation doesn't exist in "this", add it.
                if (thisTranslation == null)
                {
                  thisColumn.String.Translations.Add(thatTranslation);
                }
                else 
                {
                  // Otherwise see if there are any changes
                  if (!thatTranslation.Value.Equals(thisTranslation.Value))
                  {
                    thisTranslation.Value = thatTranslation.Value;
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
