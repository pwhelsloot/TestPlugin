using AMCS.Data.Server.SQL.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Fetch
{
  internal class SQLFetchCriteriaQueryBuilder
  {
    private readonly SQLCriteriaQueryBuilder queryBuilder;
    private readonly FetchInfo fetchInfo;

    public FetchInfo FetchInfo => fetchInfo;

    public SQLFetchCriteriaQueryBuilder(ICriteria criteria, CriteriaQueryType queryType)
    {
      var criteriaBuilder = (CriteriaBuilder)criteria;

      if (criteriaBuilder.FetchPaths != null && criteriaBuilder.FetchPaths.Any())
      {
        if (queryType == CriteriaQueryType.Count || queryType == CriteriaQueryType.Exists)
          throw new ArgumentException("Cannot mix Fetch with Count or Exists");
        if (criteria.MaxResults.HasValue)
          throw new ArgumentException("Cannot mix Fetch with MaxResults (yet)");
        this.fetchInfo = FetchInfoCreator.Create(criteriaBuilder.EntityType, criteriaBuilder.FetchPaths);

        this.queryBuilder = new SQLCriteriaQueryBuilder(criteria, queryType, FetchInfo.MainTablePrefix);
      }
      else
      {
        this.queryBuilder = new SQLCriteriaQueryBuilder(criteria, queryType);
      }
    }

    public string GetSql()
    {
      return (FetchInfo == null)
        ? queryBuilder.GetSql()
        : GetFetchSql();
    }

    public void SetParameters(ISQLQuery query) => queryBuilder.SetParameters(query);

    private string GetFetchSql()
    {
      string where = GetWhere();

      var sql = new SQLTextBuilder();

      for (int groupIndex = 0; groupIndex < fetchInfo.GroupCount; ++groupIndex)
      {
        if (groupIndex > 0)
        {
          sql
            .Text(Environment.NewLine)
            .Text("UNION ALL")
            .Text(Environment.NewLine);
        }

        WriteSelect(sql, groupIndex);
        WriteFrom(sql, groupIndex);
        sql.Text(where);
      }

      string result = sql.ToString();
      return result;
    }


    private void WriteSelect(SQLTextBuilder sql, int groupIndex)
    {
      sql.Text("SELECT ");
      FetchInfo.WriteSelectFields(sql, groupIndex);
      sql.Text(Environment.NewLine);
    }

    private void WriteFrom(SQLTextBuilder sql, int groupIndex)
    {
      sql.Text("FROM ");
      FetchInfo.WriteFromSource(sql, groupIndex);
      sql.Text(Environment.NewLine);
    }

    private string GetWhere()
    {
      if (!queryBuilder.HasWhere())
        return "";

      var sql = new SQLTextBuilder();
      queryBuilder.WriteWhere(sql);
      return sql.ToString();
    }
  }
}
