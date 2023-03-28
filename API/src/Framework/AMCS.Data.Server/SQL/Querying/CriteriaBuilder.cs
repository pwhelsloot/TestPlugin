using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  internal class CriteriaBuilder : ICriteria
  {
    public IList<IExpression> Expressions { get; } = new List<IExpression>();

    public IList<IOrder> Orders { get; } = new List<IOrder>();

    public IList<string> FetchPaths { get; private set; }

    public Type EntityType { get; }

    public int? FirstResult { get; private set; }

    public int? MaxResults { get; private set; }

    public bool IncludeDeleted { get; private set; }

    public IFieldMap FieldMap { get; private set; }

    public CriteriaBuilder(Type entityType)
    {
      EntityType = entityType;
    }

    public ICriteria Add(IExpression expression)
    {
      Expressions.Add(expression);

      return this;
    }

    public ICriteria SetFirstResult(int firstResult)
    {
      FirstResult = firstResult;

      return this;
    }

    public ICriteria SetMaxResults(int maxResults)
    {
      MaxResults = maxResults;

      return this;
    }

    public ICriteria SetIncludeDeleted(bool includeDeleted)
    {
      IncludeDeleted = includeDeleted;

      return this;
    }

    public ICriteria SetFieldMap(IFieldMap fieldMap)
    {
      FieldMap = fieldMap;

      return this;
    }

    public ICriteria Order(string field, OrderDirection direction)
    {
      Orders.Add(new FieldOrder(field, direction));

      return this;
    }

    public ICriteria Clone()
    {
      var result = new CriteriaBuilder(EntityType)
      {
        FirstResult = FirstResult,
        IncludeDeleted = IncludeDeleted,
        MaxResults = MaxResults,
        FieldMap = FieldMap
      };

      ((List<IExpression>)result.Expressions).AddRange(Expressions);
      ((List<IOrder>)result.Orders).AddRange(Orders);
      if (FetchPaths != null)
        result.FetchPaths = new List<string>(FetchPaths);

      return result;
    }

    public ICriteria Fetch(string fetchPath)
    {
      if (FetchPaths == null)
        FetchPaths = new List<string>();
      FetchPaths.Add(fetchPath);
      return this;
    }
  }
}
