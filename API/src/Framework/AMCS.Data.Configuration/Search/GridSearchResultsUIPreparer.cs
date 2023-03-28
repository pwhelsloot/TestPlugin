//-----------------------------------------------------------------------------
// <copyright file="GridSearchResultsUIPreparer.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Search
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration.Mapping;
  using AMCS.Data.Configuration.Mapping.Translate;
  using AMCS.Data.Entity.Search;

  /// <summary>
  /// Prepared Grid Search Results for outside world. In particular applies translations or defaults.
  /// </summary>
  public static class GridSearchResultsUIPreparer
  {
    /// <summary>
    /// Prepares the search results for UI.
    /// </summary>
    /// <param name="searchResultsId">The search results id.</param>
    /// <param name="translator">The translator.</param>
    /// <param name="result">The result.</param>
    public static void PrepareSearchResultsForUI(string searchResultsId, SearchResultTranslator translator, GridSearchResultsEntity result)
    {
      List<ISearchResultColumn> columns = new List<ISearchResultColumn>();

      bool translationExists = false;

      try
      {
        columns = (List<ISearchResultColumn>)translator.GetColumns();
        translationExists = true;
      }
      catch (Exception)
      {
        // Can't get translation build up a column list using defaults
        foreach (GridColumnDefinition gridColumnDefinition in result.ColumnDefinitions)
        {
          SearchResultColumn searchResultColumn = new SearchResultColumn();
          searchResultColumn.Width = -99;
          searchResultColumn.Editable = false;

          // Obviously this could be improved if need be.
          if (gridColumnDefinition.DataType.Contains("DateTime"))
          {
            searchResultColumn.StringFormat = "d";
          }
          else if (gridColumnDefinition.DataType.ToLower().Contains("decimal") && (gridColumnDefinition.Path.Contains("Cost") || gridColumnDefinition.Path.Contains("Charge") || gridColumnDefinition.Path.Contains("Price") || gridColumnDefinition.Path.Contains("Bill")))
          {
            searchResultColumn.StringFormat = "N2";
          }
          
          searchResultColumn.DisplayType = 0; // e.g. nornal
          searchResultColumn.ColumnName = gridColumnDefinition.Path;
          searchResultColumn.AggregateFunctions = new ITranslatableAggregate[] { };

          columns.Add(searchResultColumn);
        }
      }

      foreach (GridColumnDefinition gridColumnDefinition in result.ColumnDefinitions)
      {
        if (columns.Count > 0 && translator != null)
        {
          ISearchResultColumn column = columns.Find(delegate(ISearchResultColumn c) { return c.ColumnName == gridColumnDefinition.Path; });

          if (column != null && !string.IsNullOrEmpty(column.ColumnName))
          {
            if (column.Width.GetValueOrDefault(0) == 0)
            {
              gridColumnDefinition.Visible = false;
            }
            else
            {
              gridColumnDefinition.Width = column.Width.Value;

              if (translationExists)
              {
                gridColumnDefinition.Header = translator.GetLocalisedColumnName(column);
              }
              else
              {
                gridColumnDefinition.Header = gridColumnDefinition.Path;
              }

              gridColumnDefinition.Editable = column.Editable;
              
              if (!string.IsNullOrWhiteSpace(column.StringFormat))
              {
                gridColumnDefinition.DataFormatString = column.StringFormat;
              }
            }

            if (column.DisplayType.GetValueOrDefault(0) > 0)
            {
              int displayType = column.DisplayType.GetValueOrDefault(0);
              GridColumnTypeEnum columnType = (GridColumnTypeEnum)displayType;
              gridColumnDefinition.ColumnType = columnType;

              if (gridColumnDefinition.ColumnType == GridColumnTypeEnum.ctBoolImage || gridColumnDefinition.ColumnType == GridColumnTypeEnum.ctTrendIndicator)
              {
                gridColumnDefinition.ImageHeight = column.ImageHeight.GetValueOrDefault(20);
                gridColumnDefinition.ImageWidth = column.ImageWidth.GetValueOrDefault(20);

                List<string> images = new List<string>();
                if (!string.IsNullOrWhiteSpace(column.ImageReferenceOne))
                {
                  images.Add(column.ImageReferenceOne);
                }

                if (!string.IsNullOrWhiteSpace(column.ImageReferenceTwo))
                {
                  images.Add(column.ImageReferenceTwo);
                }

                gridColumnDefinition.Images = images.ToArray();
              }
            }

            gridColumnDefinition.AggregateFunctions = (from ITranslatableAggregate tA in column.AggregateFunctions
                                      select new AggregateFunctionDescriptor
                                      {
                                        Type = tA.Type,
                                        Caption = translationExists ? translator.GetLocalisedString(tA.Caption) : tA.Caption.ToString(),
                                        StringFormat = tA.StringFormat
                                      }).ToArray();

            result.AuditTable = translationExists ? translator.AuditTable : null;
            result.AuditQuery = translationExists ? translator.AuditQuery : null;
            result.AuditKeyField = translationExists ? translator.AuditKeyField : null;
            result.SearchResultId = searchResultsId;
          }
          else
          {
            // We don't know the column hide
            gridColumnDefinition.Visible = false;
          }
        }
      }
    }
  }
}