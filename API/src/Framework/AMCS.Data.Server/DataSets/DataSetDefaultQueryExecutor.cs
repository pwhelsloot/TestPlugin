using System;
using AMCS.ApiService.Filtering;
using AMCS.Data.Entity;
using AMCS.Data.Server.DataSets.FilterExpressions;
using AMCS.Data.Server.SQL.Querying;
using AMCS.Data.Support;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetDefaultQueryExecutor : IDataSetQueryExecutor
  {
    public const int MaxResults = 1000;

    public DataSetQueryResult Query(ISessionToken userId, DataSetQuery query, DataSetQueryCursor cursor, int? maxResults, IDataSession session)
    {
      if (query.DataSet.EntityType == null)
        throw new InvalidOperationException("The default query implementation requires that an entity object type is mapped");

      int actualMaxResults = Math.Min(maxResults ?? MaxResults, MaxResults);

      var mapper = DataServices.Resolve<IEntityObjectMapper>();
      var propertyMapper = CreatePropertyMapper(query.DataSet.Type, query.DataSet.EntityType, mapper);
      var filter = DataSetFilterExpressionUtils.GetFilter(query.Expressions, propertyMapper);

      if (cursor != null)
      {
        filter.Expressions.Add(new FilterBinaryExpression(
          FilterBinaryOperator.Gt,
          propertyMapper(query.DataSet.KeyColumn.Property.Name),
          new FilterValue(FilterValueKind.Integer, cursor.Id)));
      }

      var criteria = CriteriaFilterParser.Parse(filter, query.DataSet.EntityType)
        .SetMaxResults(actualMaxResults)
        .Order(propertyMapper(query.DataSet.KeyColumn.Property.Name), OrderDirection.Ascending);

      var result = BusinessServiceManager.GetService(query.DataSet.EntityType).GetAllByCriteria(userId, criteria, session);
      var mapped = mapper.MapList<IDataSetRecord>(result, query.DataSet.Type);

      DataSetQueryCursor resultCursor = null;

      if (mapped.Count >= actualMaxResults)
      {
        int maxId = 0;

        foreach (var dto in mapped)
        {
          maxId = Math.Max(maxId, dto.GetId());
        }

        resultCursor = new DataSetQueryCursor(maxId);
      }

      var table = new DataSetTable(query.DataSet, query.Columns);

      table.Records.AddRange(mapped);

      return new DataSetQueryResult(table, resultCursor);
    }

    private Func<string, string> CreatePropertyMapper(Type sourceType, Type targetType, IEntityObjectMapper mapper)
    {
      var sourceAccessor = EntityObjectAccessor.ForType(sourceType);

      return name =>
      {
        var property = sourceAccessor.GetProperty(name);
        if (property != null && mapper.TryMapProperty(property, targetType, out var targetProperty))
          return targetProperty.Name;

        return name;
      };
    }
  }
}
