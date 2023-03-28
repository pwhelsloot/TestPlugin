using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public interface ICriteria
  {
    IList<IExpression> Expressions { get; }

    IList<IOrder> Orders { get; }

    Type EntityType { get; }

    int? FirstResult { get; }

    int? MaxResults { get; }

    bool IncludeDeleted { get; }

    IFieldMap FieldMap { get; }

    ICriteria Add(IExpression expression);

    ICriteria Order(string field, OrderDirection direction);

    /// <summary>
    /// Sets the first row to be retrieved.
    /// </summary>
    /// <param name="firstResult">The first row to be retrieved offset from 0.</param>
    ICriteria SetFirstResult(int firstResult);

    /// <summary>
    /// Sets the maximum number of rows to be retrieved.
    /// </summary>
    /// <param name="maxResults">The maximum number of rows to be retrieved.</param>
    ICriteria SetMaxResults(int maxResults);

    ICriteria SetIncludeDeleted(bool includeDeleted);

    ICriteria SetFieldMap(IFieldMap fieldMap);

    ICriteria Clone();

    ICriteria Fetch(string fetchPath);
  }
}
