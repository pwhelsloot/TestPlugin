//-----------------------------------------------------------------------------
// <copyright file="SearchResultTranslator.cs" company="AMCS Group">
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

  public class SearchResultTranslator : Translator
  {
    private string searchResultId;

    private IMappingManager mappingManager;
    private ISearchResultMapping mapping;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResultTranslator"/> class.
    /// </summary>
    /// <param name="searchResultId">The search result id.</param>
    /// <param name="searchResultMappingManager">The search result mapping manager.</param>
    public SearchResultTranslator(string searchResultId, IMappingManager searchResultMappingManager) : this(searchResultId, searchResultMappingManager, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResultTranslator"/> class.
    /// </summary>
    /// <param name="searchResultId">The search result id.</param>
    /// <param name="searchResultMappingManager">The search result mapping manager.</param>
    /// <param name="allowUnmapped">if set to <c>true</c> [allow unmapped].</param>
    public SearchResultTranslator(string searchResultId, IMappingManager searchResultMappingManager, bool allowUnmapped)
      : base()
    {
      this.searchResultId = searchResultId;
      this.mappingManager = searchResultMappingManager;

      if (!this.mappingManager.IsMapped(searchResultId) && !allowUnmapped)
      {
        throw new Exception("No mapping for search result '" + searchResultId + "'.");
      }
        
      this.mapping = (ISearchResultMapping)this.mappingManager.GetMapping(searchResultId);
    }

    /// <summary>
    /// Gets the audit table.
    /// </summary>
    /// <value>The audit table.</value>
    public string AuditTable
    {
      get { return this.mapping.AuditTable; }
    }

    /// <summary>
    /// Gets the audit query.
    /// </summary>
    /// <value>The audit query.</value>
    public string AuditQuery
    {
      get { return this.mapping.AuditQuery; }
    }

    /// <summary>
    /// Gets the audit key field.
    /// </summary>
    /// <value>The audit key field.</value>
    public string AuditKeyField
    {
      get { return this.mapping.AuditKeyField; }
    }

    /// <summary>
    /// Gets the search result id.
    /// </summary>
    /// <value>The search result id.</value>
    public string SearchResultId
    {
      get { return this.searchResultId; }
    }

    public IList<ISearchResultColumn> GetColumns()
    {
      if (this.mapping == null)
      {
        throw new Exception("No key columns defined for search result mapping.");
      }

      if (this.mapping.Columns == null)
      {
        throw new Exception("No key columns defined for search result mapping '" + this.mapping.Id + "'.");
      }

      return this.mapping.Columns;
    }

    public ISearchResultColumn GetColumn(string columnName)
    {
      ISearchResultColumn column = this.mapping.Columns.FirstOrDefault(c => c.ColumnName.Equals(columnName));
      if (column == null)
      {
        throw new Exception("Column '" + columnName + "' not defined in search result mapping '" + this.mapping.Id + "'.");
      }
        
      return column;
    }

    public ISearchResultColumn GetKeyColumn()
    {
      ISearchResultColumn column = this.mapping.Columns.FirstOrDefault(c => c.IsKey);
      if (column == null)
      {
        throw new Exception("No key column defined in search result mapping '" + this.mapping.Id + "'.");
      }
        
      return column;
    }

    public int? GetColumnWidth(string columnName)
    {
      return this.GetColumn(columnName).Width;
    }

    public string GetLocalisedString(ITranslatableLocaleString translatableString)
    {
      if (CultureInfo.CurrentUICulture.Name.Equals(translatableString.Locale))
      {
        return translatableString.Value;
      }

      ILocaleString translation = translatableString.Translations.FirstOrDefault(t => t.Locale.Equals(CultureInfo.CurrentUICulture.Name));
      if (translation == null || string.IsNullOrWhiteSpace(translation.Value))
      {
        translation = this.GetFallbackString(CultureInfo.CurrentUICulture.Name, translatableString);
      }

      // Should always succeed because of GetFallbackString(...)
      if (translation != null && !string.IsNullOrWhiteSpace(translation.Value))
      {
        return translation.Value;
      }

      // If the translation can't be found return the default
      return translatableString.Value;
    }

    public string GetLocalisedColumnName(ISearchResultColumn column)
    {
      if (column.String == null)
      {
        return string.Empty;
      }

      return this.GetLocalisedString(column.String);
    }

    public string GetLocalisedColumnName(string columnName)
    {
      return this.GetLocalisedColumnName(this.GetColumn(columnName));
    }
  }
}