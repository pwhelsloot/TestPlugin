using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.Filters;
using AMCS.Data.Server.DataSets.Restrictions;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [DebuggerDisplay("Name = {Name}, Label = {Label}, Type = {Type}, Columns = {Columns.Count}")]
  [JsonConverter(typeof(DataSetConverter))]
  public class DataSet
  {
    private readonly Dictionary<string, DataSetColumn> columnsByName;

    public string Name { get; }

    public string Label { get; }

    public Type Type { get; }

    public Type EntityType { get; }

    public Func<IDataSetQueryExecutor> QueryExecutorFactory { get; }

    public Func<IDataSetImportExecutor> ImportExecutorFactory { get; }

    public DataSetColumn KeyColumn { get; }

    public DataSetColumn DisplayColumn { get; }

    public IList<DataSetColumn> Columns { get; }

    public IList<DataSetRestriction> Restrictions { get; private set; }

    public IList<DataSetFilter> Filters { get; private set; }

    public IList<DataSetRelationship> Relationships { get; private set; }

    public DataSet(string name, string label, Type type, Type entityType, Func<IDataSetQueryExecutor> queryExecutorFactory, Func<IDataSetImportExecutor> importExecutorFactory, DataSetColumn displayColumn, DataSetColumn keyColumn, IList<DataSetColumn> columns)
    {
      Name = name;
      Label = label;
      Type = type;
      EntityType = entityType;
      QueryExecutorFactory = queryExecutorFactory;
      ImportExecutorFactory = importExecutorFactory;
      DisplayColumn = displayColumn;
      KeyColumn = keyColumn;
      Columns = columns;

      columnsByName = columns.ToDictionary(p => p.Property.Name, p => p);
    }

    public DataSetColumn GetColumn(string name)
    {
      var column = FindColumn(name);
      if (column == null)
        throw new ArgumentException($"Unknown column '{Name}.{name}'");

      return column;
    }

    public DataSetColumn FindColumn(string name)
    {
      columnsByName.TryGetValue(name, out var column);
      return column;
    }

    public virtual DataSetQueryResult Query(ISessionToken userId, DataSetQuery query, DataSetQueryCursor cursor, int? maxResults, IDataSession session)
    {
      ValidateQuery(query);

      return QueryExecutorFactory().Query(userId, query, cursor, maxResults, session);
    }

    private void ValidateQuery(DataSetQuery query)
    {
      foreach (var expression in query.Expressions)
      {
        bool anyMatch = false;

        foreach (var filter in query.DataSet.Filters)
        {
          anyMatch =
            expression.Column == filter.Column &&
            filter.IsMatch(expression);

          if (anyMatch)
            break;
        }

        if (!anyMatch)
          throw new DataSetException($"No filter is available matching the expression on column '{expression.Column.Label}'");
      }
    }

    internal void Complete(List<DataSetRestriction> restrictions, List<DataSetFilter> filters)
    {
      Restrictions = restrictions;
      Filters = filters;

      Relationships = new ReadOnlyCollection<DataSetRelationship>(
        Restrictions.OfType<DataSetReferenceRestriction>()
          .Select(p => new DataSetRelationship(this, p.Column, p.Referenced))
          .ToList()
      );
    }
  }
}
