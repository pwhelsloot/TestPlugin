//-----------------------------------------------------------------------------
// <copyright file="SQLSearchDataAccessProvider.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration.Mapping.Translate;
using AMCS.Data.Configuration.Search;
using AMCS.Data.Entity;
using AMCS.Data.Entity.Search;
using Microsoft.SqlServer.Types;

namespace AMCS.Data.Server.SQL
{
  public class SQLSearchDataAccessProvider : ISearchDataAccessProvider
  {
    public const string SearchProcedurePrefix = "spS_";

    /// <summary>
    /// Gets the search results from reader.
    /// </summary>
    /// <param name="searchResultsId">The search results id.</param>
    /// <param name="rdr">The RDR.</param>
    /// <returns></returns>
    public SearchResultsEntity GetSearchResultsFromReader(string searchResultsId, IDataReader rdr)
    {
      SearchResultsEntity results = new SearchResultsEntity();
      results.Id = searchResultsId;
      results.Columns = new SerialisableKeyValuePair<string, string>[rdr.FieldCount];

      DataTable schemaTable = rdr.GetSchemaTable();

      for (int i = 0; i < rdr.FieldCount; i++)
      {
        SerialisableKeyValuePair<string, string> col = new SerialisableKeyValuePair<string, string>();
        col.Key = rdr.GetName(i);
        col.Value = (schemaTable.Rows[i]["DataType"] as Type).FullName;

        results.Columns[i] = col;
      }

      while (rdr.Read())
      {
        object[] row = new object[rdr.FieldCount];
        for (int i = 0; i < rdr.FieldCount; i++)
        {
          object value = rdr[i];
          if (value is DBNull)
            value = null;
          row[i] = value;
        }

        results.Data.Add(row);
      }
      return results;
    }

    /// <summary>
    /// Gets the grid search results from reader.
    /// </summary>
    /// <param name="searchResultsId">The search results id.</param>
    /// <param name="rdr">The RDR.</param>
    /// <returns></returns>
    public GridSearchResultsEntity GetGridSearchResultsFromReader(string searchResultsId, IDataReader rdr, IList<string> ignoreColumns = null)
    {
      GridSearchResultsEntity result = new GridSearchResultsEntity();
      result.Id = searchResultsId;

      DataTable schemaTable = rdr.GetSchemaTable();

      for (int i = 0; i < rdr.FieldCount; i++)
      {
        if (ignoreColumns != null && ignoreColumns.Contains(rdr.GetName(i))) continue;

        bool isNullable = (bool)schemaTable.Rows[i]["AllowDBNull"];
        Type dataType = schemaTable.Rows[i]["DataType"] as Type;

        if (dataType == null)
        {
          if (schemaTable.Rows[i]["DataTypeName"].ToString().Split('.').Last().ToLowerInvariant() == "geography")
          {
            dataType = typeof(SqlGeography);
          }
        }

        // If column is NULLable
        // and datatype of column can be Nullable<>
        // and datatype is not already Nullable
        if (isNullable && dataType.IsValueType && Nullable.GetUnderlyingType(dataType) == null)
        {
          // Make datatype Nullable<>
          dataType = typeof(Nullable<>).MakeGenericType(dataType);
        }

        GridColumnDefinition columnDefinition = new GridColumnDefinition
        {
          ParentId = searchResultsId,
          Path = rdr.GetName(i),
          Header = rdr.GetName(i),
          DataType = dataType.FullName,
          Visible = true
        };
        result.ColumnDefinitions.Add(columnDefinition);
      }

      while (rdr.Read())
      {
        dynamic row = new SerializableDynamicObject();
        for (int i = 0; i < rdr.FieldCount; i++)
        {
          if (ignoreColumns != null && ignoreColumns.Contains(rdr.GetName(i))) continue;

          object value;

          var fieldType = rdr.GetDataTypeName(i).Split('.').Last();
          if (fieldType.ToLowerInvariant() == "geography")
          {
            value = SQLGeographyHelper.GetSqlGeographyData(rdr, i);
          }
          else
            value = rdr[i];

          if (value is DBNull)
            value = null;
          row[rdr.GetName(i)] = value;
        }
        result.Collection.Add(row);
      }

      return result;
    }

    /// <summary>
    /// Gets the grid search results from reader.
    /// These will already have been translated if a translation file exists on the server
    /// </summary>
    /// <param name="searchResultsId">The search results id.</param>
    /// <param name="rdr">The RDR.</param>
    /// <returns></returns>
    public GridSearchResultsEntity GetDynamicGridSearchResultsFromReader(string searchResultsId, IDataReader rdr, IList<string> ignoreColumns = null)
    {
      GridSearchResultsEntity result = GetGridSearchResultsFromReader(searchResultsId, rdr, ignoreColumns);

      SearchResultTranslator translator = new SearchResultTranslator(searchResultsId, DataServices.Resolve<MappingManagerAccessor>().SearchResultMappingManager, true);

      // this will handle translation and the case where translation fails
      GridSearchResultsUIPreparer.PrepareSearchResultsForUI(searchResultsId, translator, result);

      return result;
    }
  }
}