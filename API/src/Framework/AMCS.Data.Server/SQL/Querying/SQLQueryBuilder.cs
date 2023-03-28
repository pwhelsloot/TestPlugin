using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public class SQLQueryBuilder
  {
    public static SQLQueryBuilder FromCriteria(ICriteria criteria, CriteriaQueryType queryType, string select = null)
    {
      var builder = new SQLCriteriaQueryBuilder(criteria, queryType);
      var query = new SQLQueryBuilder();
      var sql = new SQLTextBuilder();

      builder.WriteSelectFields(sql, select);
      query.AddSelect(sql.ToString());

      sql.Clear();

      builder.WriteFromSource(sql);
      query.AddFrom(sql.ToString());
      sql.Clear();

      if (builder.HasWhere())
      {
        builder.WriteWhereCondition(sql);
        query.AddWhere(sql.ToString());
        sql.Clear();
      }
      if (builder.HasOrderBy())
      {
        builder.WriteOrderByExpression(sql);
        query.AddOrderBy(sql.ToString());
        sql.Clear();
      }

      foreach (var parameter in builder.Parameters)
      {
        query.Parameters.Add(parameter);
      }

      return query;
    }

    // This class contains 7 lists to keep all the different SQL snippets
    // to build up the final query. An alternative implementation was attempted
    // that kept all data in a single array with a snippet type. The results
    // of benchmarking this implementation were a 15% decrease in memory usage
    // and a 17% slowdown. See http://dw-tfs-03:8080/tfs/AMCS/DevOps/_git/ElemosBenchmarks
    // and its README for more information.

    private List<string> prologs;
    private List<string> withs;
    private List<string> selects;
    private List<string> froms;
    private List<string> wheres;
    private List<string> groupBys;
    private List<string> havings;
    private List<string> orderBys;

    public SQLQueryParameterCollection Parameters { get; } = new SQLQueryParameterCollection();

    public void SetProlog(string prolog)
    {
      ClearProlog();
      AddProlog(prolog);
    }

    public void ClearProlog()
    {
      prologs?.Clear();
    }

    public void AddProlog(string prolog)
    {
      if (prologs == null)
        prologs = new List<string>();
      prologs.Add(prolog);
    }

    public void SetWith(string with)
    {
      ClearWith();
      AddWith(with);
    }

    public void ClearWith()
    {
      withs?.Clear();
    }

    public void AddWith(string with)
    {
      if (withs == null)
        withs = new List<string>();
      withs.Add(with);
    }

    public void SetSelect(string select)
    {
      ClearSelect();
      AddSelect(select);
    }

    public void ClearSelect()
    {
      selects?.Clear();
    }

    public void AddSelect(string select)
    {
      if (selects == null)
        selects = new List<string>();
      selects.Add(select);
    }

    public void SetFrom(string from)
    {
      ClearFrom();
      AddFrom(from);
    }

    public void ClearFrom()
    {
      froms?.Clear();
    }

    public void AddFrom(string from)
    {
      if (froms == null)
        froms = new List<string>();
      froms.Add(from);
    }

    [Obsolete("Use ResetWhere instead")]
    public void SetWhere(string where)
    {
      ClearWhere();
      AddWhere(where);
    }

    public void ResetWhere(string where)
    {
      ClearWhere();
      AddWhere(where);
    }

    public void ClearWhere()
    {
      wheres?.Clear();
    }

    public void AddWhere(string where)
    {
      if (wheres == null)
        wheres = new List<string>();
      wheres.Add(where);
    }

    public void SetGroupBy(string groupBy)
    {
      ClearGroupBy();
      AddGroupBy(groupBy);
    }

    public void ClearGroupBy()
    {
      groupBys?.Clear();
    }

    public void AddGroupBy(string groupBy)
    {
      if (groupBys == null)
        groupBys = new List<string>();
      groupBys.Add(groupBy);
    }

    public void SetHaving(string having)
    {
      ClearHaving();
      AddHaving(having);
    }

    public void ClearHaving()
    {
      havings?.Clear();
    }

    public void AddHaving(string having)
    {
      if (havings == null)
        havings = new List<string>();
      havings.Add(having);
    }

    public void SetOrderBy(string orderBy)
    {
      ClearOrderBy();
      AddOrderBy(orderBy);
    }

    public void ClearOrderBy()
    {
      orderBys?.Clear();
    }

    public void AddOrderBy(string orderBy)
    {
      if (orderBys == null)
        orderBys = new List<string>();
      orderBys.Add(orderBy);
    }

    public void Write(SQLTextBuilder sql)
    {
      if (prologs?.Count > 0)
        Write(sql, string.Empty, prologs, Environment.NewLine);
      if (withs?.Count > 0)
        Write(sql, "WITH", withs, "," + Environment.NewLine);
      if (selects?.Count > 0)
        Write(sql, "SELECT", selects, ", ");
      if (froms?.Count > 0)
        Write(sql, "FROM", froms, " ");
      if (wheres?.Count > 0)
        Write(sql, "WHERE", wheres, " AND ");
      if (groupBys?.Count > 0)
        Write(sql, "GROUP BY", groupBys, ", ");
      if (havings?.Count > 0)
        Write(sql, "HAVING", havings, " AND ");
      if (orderBys?.Count > 0)
        Write(sql, "ORDER BY", orderBys, ", ");
    }

    private void Write(SQLTextBuilder sql, string prefix, List<string> items, string delimiter)
    {
      if (sql.Length > 0)
        sql.Text(Environment.NewLine);

      sql.Text(prefix).Text(" ");

      for (var i = 0; i < items.Count; i++)
      {
        if (i > 0)
          sql.Text(delimiter);
        sql.Text(items[i]);
      }
    }

    public override string ToString()
    {
      var sql = new SQLTextBuilder();
      Write(sql);
      return sql.ToString();
    }
  }
}
