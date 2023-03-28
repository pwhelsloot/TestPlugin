using AMCS.Data.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.Server.SQL.Fetch
{
  /// <summary>
  /// Stores all information about fetch rules that is needed by the framework for creating the query and loading from the DB.
  /// </summary>
  public class FetchInfo
  {
    internal const string MainTablePrefix = "[j0].";

    internal IList<FetchJoin> Joins { get; }
    internal int GroupCount { get; }

    internal FetchInfo(IList<FetchJoin> joins, int groupCount)
    {
      Joins = joins;
      GroupCount = groupCount;
    }

    internal void WriteFromSource(SQLTextBuilder sql, int groupIndex)
    {
      foreach (var join in Joins)
      {
        join.WriteFromJoin(sql, groupIndex);
      }
    }

    internal void WriteSelectFields(SQLTextBuilder sql, int groupIndex)
    {
      foreach (var join in Joins)
      {
        join.WriteSelectFields(sql, groupIndex);
      }
    }    
  }
}
