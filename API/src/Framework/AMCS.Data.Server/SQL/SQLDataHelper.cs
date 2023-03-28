//-----------------------------------------------------------------------------
// <copyright file="SQLDataHelper.cs" company="AMCS Group">
//   Copyright © 2013 AMCS Group. All rights reserved.
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
using Microsoft.SqlServer.Server;

namespace AMCS.Data.Server.SQL
{
  public static class SQLDataHelper
  {
    public static IList<SqlDataRecord> ConvertIdListToTableType(string name, IList<int> list)
    {
      List<SqlDataRecord> ids = new List<SqlDataRecord>();

      SqlMetaData[] metaData = { new SqlMetaData(name, SqlDbType.Int) };
      if (list != null && list.Count > 0)
      {
        foreach (int id in list)
        {
          SqlDataRecord row = new SqlDataRecord(metaData);
          row.SetInt32(0, id);
          ids.Add(row);
          return ids;
        }
      }
      return null;
    
    }
  }
}
