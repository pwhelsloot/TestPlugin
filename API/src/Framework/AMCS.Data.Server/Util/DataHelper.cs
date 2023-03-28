//-----------------------------------------------------------------------------
// <copyright file="DataHelper.cs" company="AMCS Group">
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

namespace AMCS.Data.Server.Util
{
  /// <summary>
  ///
  /// </summary>
  public static class DataHelper
  {
    [Obsolete("Use new ORM system")]
    public static DataTable IdsToDataTable(IEnumerable<int> ids)
    {
      DataTable table = new DataTable();
      table.Columns.Add("Id", typeof(int));
      if (ids != null)
      {
        foreach (int id in ids)
          table.Rows.Add(id);
      }
      return table;
    }

    /// <summary>
    /// Default join that takes an IEnumerable list and just takes the ToString of each item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="separator">The separator.</param>
    /// <param name="list">The list.</param>
    /// <returns></returns>
    public static string Join<T>(string separator, IEnumerable<T> list)
    {
      return Join<T>(separator, list, delegate(T o) { return o.ToString(); });
    }

    /// <summary>
    /// Join that takes an IEnumerable list that uses a converter to convert the type to a string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="separator">The separator.</param>
    /// <param name="list">The list.</param>
    /// <param name="converter">The converter.</param>
    /// <returns></returns>
    public static string Join<T>(string separator, IEnumerable<T> list, Converter<T, string> converter)
    {
      StringBuilder sb = new StringBuilder();
      foreach (T t in list)
      {
        if (sb.Length != 0) sb.Append(separator);
        sb.Append(converter(t));
      }
      return sb.ToString();
    }
  }
}