using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AMCS.Data.Entity;
using AMCS.Data.Server.SQL.Fetch;

namespace AMCS.Data.Server.SQL.Querying
{
  public class SQLCriteriaQueryBuilder
  {
    private readonly CriteriaBuilder criteria;
    private readonly CriteriaQueryType queryType;
    private readonly ParameterBuilder parameters;
    private readonly EntityObjectAccessor accessor;
    private readonly List<IExpression> expressions = new List<IExpression>();
    private readonly ISearchExpression search;
    private readonly SearchMode searchMode;
    private readonly string mainTablePrefix;

    public SQLQueryParameterCollection Parameters { get; } = new SQLQueryParameterCollection();

    public SQLCriteriaQueryBuilder(ICriteria criteria, CriteriaQueryType queryType, string mainTablePrefix = null)
    {
      this.criteria = (CriteriaBuilder)criteria;
      this.queryType = queryType;
      this.accessor = EntityObjectAccessor.ForType(this.criteria.EntityType);
      this.parameters = new ParameterBuilder(Parameters);
      this.mainTablePrefix = mainTablePrefix;

      if (mainTablePrefix == null && this.criteria.FetchPaths != null)
        throw new ArgumentException($"Cannot to use {nameof(SQLCriteriaQueryBuilder)} directly on query that includes Fetch. Use {nameof(SQLFetchCriteriaQueryBuilder)} instead");

      foreach (var expression in this.criteria.Expressions)
      {
        if (expression is ISearchExpression search)
        {
          this.search = search;
          searchMode = this.criteria.Orders.Count > 0 ? SearchMode.Simple : SearchMode.Ranked;
        }
        else
        {
          expressions.Add(expression);
        }
      }
    }

    public bool HasOrderBy()
    {
      // On count queries, we don't want an order by.

      if (queryType == CriteriaQueryType.Count || queryType == CriteriaQueryType.Exists)
        return false;

      // If we don't have orders, but do have a first/max result, we need
      // an order by clause since FETCH/OFFSET requires it.
      //
      // Search also implies an order by because if no order is specify,
      // we sort by rank.

      return
        criteria.Orders.Count > 0 ||
        criteria.FirstResult.HasValue ||
        criteria.MaxResults.HasValue ||
        searchMode == SearchMode.Ranked;
    }

    public bool HasWhere()
    {
      return
        expressions.Count > 0 ||
        (accessor.CanUndelete && !criteria.IncludeDeleted) ||
        searchMode == SearchMode.Simple;
    }

    public string GetSql()
    {
      var sql = new SQLTextBuilder();

      WriteSelect(sql);

      WriteFrom(sql);

      if (HasWhere())
        WriteWhere(sql);

      if (HasOrderBy())
        WriteOrderBy(sql);

      return sql.ToString();
    }

    public string GetSelect()
    {
      var sql = new SQLTextBuilder();
      WriteSelect(sql);
      return sql.ToString();
    }

    public string GetSelectFields()
    {
      var sql = new SQLTextBuilder();
      WriteSelectFields(sql);
      return sql.ToString();
    }

    private void WriteSelect(SQLTextBuilder sql)
    {
      sql.Text("SELECT ");
      WriteSelectFields(sql);
      sql.Text(Environment.NewLine);
    }

    public void WriteSelectFields(SQLTextBuilder sql)
    {
      switch (queryType)
      {
        case CriteriaQueryType.Select:
          sql.Text("*");
          break;

        case CriteriaQueryType.Count:
          sql.Text("COUNT(*)");
          break;

        case CriteriaQueryType.Exists:
          sql.Text("TOP 1 1");
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void WriteSelectFields(SQLTextBuilder sql, string select = null)
    {
      if (string.IsNullOrEmpty(select))
      {
        this.WriteSelectFields(sql);
      }
      else
      {
        sql.Text(select);

        switch (searchMode)
        {
          case SearchMode.Ranked:
            // Ranked searches will be ordered by rank, so also want to include in SELECT in case needed
            sql.Text(", " + GetIndexKeyAlias()).Text(".").Name("RANK");
            break;

          default:
            break;
        }
      }
    }

    private void WriteLike(SQLTextBuilder sql, string field, Like like)
    {
      var sb = new StringBuilder();
      bool hadEscape = false;
      string value = like.Value;

      foreach (char c in value)
      {
        switch (c)
        {
          case '%':
          case '_':
          case '[':
            hadEscape = true;
            sb.Append('!');
            break;
          case '!':
            // Don't set hadEscape since we won't need to escape this if
            // there are no other characters to escape.
            sb.Append('!');
            break;
        }

        sb.Append(c);
      }

      if (hadEscape)
        value = sb.ToString();

      switch (like.Kind)
      {
        case LikeKind.Contains:
          value = "%" + value + "%";
          break;
        case LikeKind.StartsWith:
          value += "%";
          break;
        case LikeKind.EndsWith:
          value = "%" + value;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      sql.ParameterName(parameters.AddParameter(field, value));

      if (hadEscape)
        sql.Text(" ESCAPE '!'");
    }

    public string GetFrom()
    {
      var sql = new SQLTextBuilder();
      WriteFrom(sql);
      return sql.ToString();
    }

    public void WriteFrom(SQLTextBuilder sql)
    {
      sql.Text("FROM ");
      WriteFromSource(sql);
      sql.Text(Environment.NewLine);
    }

    public string GetFromSource()
    {
      var sql = new SQLTextBuilder();
      WriteFromSource(sql);
      return sql.ToString();
    }

    public void WriteFromSource(SQLTextBuilder sql)
    {
      sql.TableName(accessor);

      switch (searchMode)
      {
        case SearchMode.Simple:
          JoinOnSearch(sql);
          break;
        case SearchMode.Ranked:
          JoinOnSearchRanked(sql);
          break;
      }
    }

    private void JoinOnSearch(SQLTextBuilder sql)
    {
      string indexAlias = GetIndexAlias();

      sql
        .Text(Environment.NewLine)
        .Text(" LEFT JOIN ")
        .TableName("search", "Index").Text(" AS ").Name(indexAlias)
        .Text(" ON ").TableName(accessor).Text(".").Name(accessor.KeyName).Text(" = ").Name(indexAlias).Text(".").Name("TableId")
        .Text(" AND ").Name(indexAlias).Text(".").Name("Table").Text(" = ").ParameterLiteral(parameters.AddParameter("SearchTable", accessor.TableNameWithSchema));
    }

    private void JoinOnSearchRanked(SQLTextBuilder sql)
    {
      JoinOnSearch(sql);

      string indexAlias = GetIndexAlias();
      string indexKeyAlias = GetIndexKeyAlias();

      sql
        .Text(Environment.NewLine)
        .Text(" INNER JOIN CONTAINSTABLE(")
        .TableName("search", "Index").Text(", ")
        .SearchRankedNames("PrimaryReference", "Text").Text(", ")
        .ParameterName(parameters.AddParameter("Search", search.Search))
        .Text(") AS ").Name(indexKeyAlias).Text(" ON ").Name(indexAlias).Text(".").Name("Id").Text(" = ").Name(indexKeyAlias).Text(".").Name("KEY");
    }

    public string GetWhere()
    {
      var sql = new SQLTextBuilder();
      WriteWhere(sql);
      return sql.ToString();
    }

    public void WriteWhere(SQLTextBuilder sql)
    {
      sql.Text("WHERE ");
      WriteWhereCondition(sql);
      sql.Text(Environment.NewLine);
    }

    public string GetWhereCondition()
    {
      var sql = new SQLTextBuilder();
      WriteWhereCondition(sql);
      return sql.ToString();
    }

    private string GetFieldName(string name)
    {
      if (criteria.FieldMap != null && criteria.FieldMap.TryMap(name, out string result))
        return result;

      var property = accessor.GetProperty(name);
      if (property == null)
        throw new SQLCriteriaException($"Invalid field name '{name}'");

      string fieldName = "[" + property.Column.ColumnName + "]";
      if (mainTablePrefix != null)
        return mainTablePrefix + fieldName;
      return fieldName;
    }

    public void WriteWhereCondition(SQLTextBuilder sql)
    {
      bool hadCondition = false;

      foreach (var expression in expressions)
      {
        if (hadCondition)
          sql.Text(" AND ");
        else
          hadCondition = true;

        switch (expression)
        {
          case IFieldExpression fieldExpression:
            AddFieldExpression(sql, fieldExpression);
            break;
          case SQLExpression sqlExpression:
            AddSQLExpression(sql, sqlExpression);
            break;
          default:
            throw new InvalidOperationException($"Unexpected expression type '{expression.GetType()}'");
        }
      }

      if (accessor.CanUndelete && !criteria.IncludeDeleted)
      {
        if (hadCondition)
          sql.Text(" AND ");
        else
          hadCondition = true;

        if (mainTablePrefix != null)
            sql.Text(mainTablePrefix);

        sql.Name("IsDeleted").Text(" = 0");
      }

      if (searchMode == SearchMode.Simple)
      {
        if (hadCondition)
          sql.Text(" AND ");
        else
          hadCondition = true;

        sql
          .Text("FREETEXT(")
          .Name(GetIndexAlias()).Text(".").Name("Text")
          .Text(", ")
          .ParameterName(parameters.AddParameter("Search", search.Search))
          .Text(")");
      }

      if (!hadCondition)
        sql.Text("1 = 1");
    }

    private void AddFieldExpression(SQLTextBuilder sql, IFieldExpression expression)
    {
      sql.Text(GetFieldName(expression.Field)).Text(" ").Text(GetComparisonOperator(expression.Comparison)).Text(" ");

      switch (expression.Comparison)
      {
        case FieldComparison.Eq:
        case FieldComparison.Ne:
        case FieldComparison.Gt:
        case FieldComparison.Ge:
        case FieldComparison.Lt:
        case FieldComparison.Le:
        case FieldComparison.Like:
          if (expression.Value is Like like)
            WriteLike(sql, expression.Field, like);
          else
            sql.ParameterName(parameters.AddParameter(expression.Field, expression.Value));
          break;

        case FieldComparison.Null:
        case FieldComparison.NotNull:
          // Field value is ignored.
          break;

        case FieldComparison.In:
          sql.Text("(");

          bool hadOne = false;
          foreach (object value in (IEnumerable)expression.Value)
          {
            if (hadOne)
              sql.Text(", ");
            else
              hadOne = true;

            sql.ParameterName(parameters.AddParameter(expression.Field, value));
          }

          if (!hadOne)
            throw new SQLCriteriaException("Expected at least one value for in expression");

          sql.Text(")");
          break;

        case FieldComparison.InSQL:
          sql.Text("(");
          WriteSQLExpression(sql, (SQLExpression)expression.Value);
          sql.Text(")");
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void AddSQLExpression(SQLTextBuilder sql, SQLExpression sqlExpression)
    {
      sql.Text("(");
      WriteSQLExpression(sql, sqlExpression);
      sql.Text(")");
    }

    private void WriteSQLExpression(SQLTextBuilder sql, SQLExpression sqlExpression)
    {
      for (var i = 0; i < sqlExpression.SQL.Parameters.Count; i++)
      {
        var parameter = sqlExpression.SQL.Parameters[i];
        object value = sqlExpression.Values[i];

        parameter.Name = parameters.AddParameter("p", value);
      }

      sql.Text(sqlExpression.SQL.ToString());
    }

    public string GetOrderBy()
    {
      var sql = new SQLTextBuilder();
      WriteOrderBy(sql);
      return sql.ToString();
    }

    public void WriteOrderBy(SQLTextBuilder sql)
    {
      sql.Text("ORDER BY ");
      WriteOrderByExpression(sql);
      sql.Text(Environment.NewLine);
    }

    public string GetOrderByExpression()
    {
      var sql = new SQLTextBuilder();
      WriteOrderByExpression(sql);
      return sql.ToString();
    }

    public void WriteOrderByExpression(SQLTextBuilder sql)
    {
      bool hadOrderBy = false;

      foreach (var order in criteria.Orders)
      {
        if (hadOrderBy)
          sql.Text(", ");
        else
          hadOrderBy = true;

        sql.Text(GetFieldName(order.Field)).Text(order.Direction == OrderDirection.Ascending ? " ASC" : " DESC");
      }

      if (searchMode == SearchMode.Ranked)
      {
        if (hadOrderBy)
          throw new InvalidOperationException();
        hadOrderBy = true;

        sql.Name(GetIndexKeyAlias()).Text(".").Name("RANK").Text(" DESC");
      }

      if (criteria.FirstResult.HasValue || criteria.MaxResults.HasValue)
      {
        // FETCH/OFFSET requires an ORDER BY. If we don't have one, we order on Id.

        if (!hadOrderBy)
        {
          sql.Name(accessor.KeyName);
          hadOrderBy = true;
        }

        // ICriteria.SetFirstResult expects a 0 offset, but OFFSET expects 1 offset.
        // That's the reason for the + 1 here.
        sql.Text(" OFFSET ").ParameterName(parameters.AddParameter("FirstResult", criteria.FirstResult.GetValueOrDefault()))
          .Text(" ROWS");

        if (criteria.MaxResults.HasValue)
          sql.Text(" FETCH FIRST ").ParameterName(parameters.AddParameter("MaxResults", criteria.MaxResults)).Text(" ROWS ONLY");
      }

      if (!hadOrderBy)
        throw new InvalidOperationException("Expected at least one order by expression");
    }

    private string GetComparisonOperator(FieldComparison comparison)
    {
      switch (comparison)
      {
        case FieldComparison.Eq:
          return "=";
        case FieldComparison.Ne:
          return "<>";
        case FieldComparison.Gt:
          return ">";
        case FieldComparison.Ge:
          return ">=";
        case FieldComparison.Lt:
          return "<";
        case FieldComparison.Le:
          return "<=";
        case FieldComparison.Null:
          return "IS NULL";
        case FieldComparison.NotNull:
          return "IS NOT NULL";
        case FieldComparison.Like:
          return "LIKE";
        case FieldComparison.In:
        case FieldComparison.InSQL:
          return "IN";
        default:
          throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
      }
    }

    private string GetIndexAlias()
    {
      return accessor.TableName + "_Index";
    }

    private string GetIndexKeyAlias()
    {
      return GetIndexAlias() + "Key";
    }

    public void SetParameters(ISQLQuery query)
    {
      foreach (var parameter in Parameters)
      {
        query.SetObject(parameter.Name, parameter.Value);
      }
    }

    private class ParameterBuilder
    {
      private readonly HashSet<string> names = new HashSet<string>();
      private readonly SQLQueryParameterCollection parameters;

      public ParameterBuilder(SQLQueryParameterCollection parameters)
      {
        this.parameters = parameters;
      }

      public string AddParameter(string fieldName, object value)
      {
        for (int i = 0;; i++)
        {
          string parameterName = "@" + fieldName;
          if (i > 0)
            parameterName += "_" + i;

          if (names.Add(parameterName))
          {
            parameters.Add(parameterName, value);
            return parameterName;
          }
        }
      }
    }

    private enum SearchMode
    {
      None,
      Simple,
      Ranked
    }
  }
}
