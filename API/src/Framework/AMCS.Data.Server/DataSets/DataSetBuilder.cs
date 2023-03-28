using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration.Mapping.Translate;
using AMCS.Data.Entity;
using AMCS.Data.Server.DataSets.Filters;
using AMCS.Data.Server.DataSets.Restrictions;

namespace AMCS.Data.Server.DataSets
{
  public abstract class DataSetBuilder
  {
    public abstract DataSet DataSet { get; }

    internal List<DataSetRestriction> GetRestrictions(IDataSetService service)
    {
      var builder = new DataSetRestrictionBuilder(DataSet);

      BuildRestrictions(builder, service);

      return builder.Restrictions;
    }

    public virtual Func<IDataSetQueryExecutor> GetQueryExecutorFactory()
    {
      return () => new DataSetDefaultQueryExecutor();
    }

    public virtual Func<IDataSetImportExecutor> GetImportExecutorFactory()
    {
      return () => new DataSetDefaultImportExecutor();
    }

    protected virtual void BuildRestrictions(DataSetRestrictionBuilder builder, IDataSetService service)
    {
      foreach (var column in DataSet.Columns)
      {
        if (column.IsMandatory)
          builder.AddNotEmpty(column);
      }
    }

    internal List<DataSetFilter> GetFilters(IDataSetService service)
    {
      var builder = new DataSetFilterBuilder(DataSet);

      BuildFilters(builder, service);

      return builder.Filters;
    }

    protected virtual void BuildFilters(DataSetFilterBuilder builder, IDataSetService service)
    {
    }
  }

  public abstract class DataSetBuilder<T> : DataSetBuilder
    where T : class, IDataSetRecord, new()
  {
    public override DataSet DataSet { get; }

    protected DataSetBuilder()
    {
      var dataSetAttribute = typeof(T).GetCustomAttribute<DataSetAttribute>();
      if (dataSetAttribute == null)
        throw new InvalidOperationException("Data set requires a DataSet attribute");

      if (typeof(EntityObject).IsAssignableFrom(typeof(T)))
        throw new InvalidOperationException("Data set DTO's cannot inherit from EntityObject");

      var columns = new List<DataSetColumn>();
      DataSetColumn displayColumn = null;
      DataSetColumn keyColumn = null;
      var accessor = EntityObjectAccessor.ForType(typeof(T));

      foreach (var property in typeof(T).GetProperties())
      {
        var dataSetColumnAttribute = property.GetCustomAttribute<DataSetColumnAttribute>();
        if (dataSetColumnAttribute == null)
          continue;

        if (!dataSetColumnAttribute.IsKeyColumn && !(property.PropertyType.IsClass || Nullable.GetUnderlyingType(property.PropertyType) != null))
          throw new InvalidOperationException("All columns except key column must be nullable");

        var column = new DataSetColumn(
          DataSetUtil.GetLocalizedString(GetType(), dataSetColumnAttribute.Label),
          accessor.GetProperty(property.Name),
          dataSetColumnAttribute.IsReadOnly,
          dataSetColumnAttribute.IsMandatory,
          dataSetColumnAttribute.IsDefault
        );

        columns.Add(column);

        if (dataSetColumnAttribute.IsDisplayColumn)
        {
          if (displayColumn != null)
            throw new InvalidOperationException("At most one display column is allowed");
          displayColumn = column;
        }
        if (dataSetColumnAttribute.IsKeyColumn)
        {
          if (keyColumn != null)
            throw new InvalidOperationException("At most one key column is allowed");
          if (column.Property.Type != typeof(int))
            throw new InvalidOperationException("Key column must be of type int");
          keyColumn = column;
        }
      }

      if (keyColumn == null)
        throw new InvalidOperationException($"No key column has been defined on '{typeof(T)}'");

      DataSet = new DataSet(
        dataSetAttribute.Name,
        DataSetUtil.GetLocalizedString(GetType(), dataSetAttribute.Label),
        typeof(T),
        dataSetAttribute.EntityType,
        GetQueryExecutorFactory(),
        GetImportExecutorFactory(),
        displayColumn,
        keyColumn,
        new ReadOnlyCollection<DataSetColumn>(columns.ToArray())
      );
    }

    
  }
}
