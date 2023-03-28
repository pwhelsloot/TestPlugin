using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.DataSets.Import;
using AMCS.Data.Server.DataSets.Restrictions;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetDefaultImportExecutor : IDataSetImportExecutor
  {
    public int? SaveRecord(ISessionToken userId, int? id, IDataSetRecord source, DataSetTable table, IList<DataSetColumn> columns, IDataSetDefaultImportIdMapper idMapper, MessageCollection messages, IDataSession dataSession)
    {
      IDataSetRecord record;

      if (id.HasValue)
      {
        record = GetRecord(userId, id.Value, table.DataSet, dataSession);
        if (record == null)
          throw new DataSetException($"Cannot find '{table.DataSet.Name}' with ID '{id}'");
      }
      else
      {
        record = CreateRecord(table.DataSet.Type);
      }

      record.SetReferenceKey(source.GetReferenceKey());

      // We always write records if we're creating them. By initializing
      // changed with true in that case, we skip any checks below.

      bool changed = !id.HasValue;

      foreach (var column in columns)
      {
        object value = column.Property.GetValue(source);

        if (
          value is int intValue &&
          idMapper.TryMapId(table.DataSet, intValue, source, column, out int mappedId)
        )
          value = mappedId;

        var property = column.Property;

        if (!changed)
        {
          var current = property.GetValue(record);
          changed = !Equals(current, value);
        }

        property.SetValue(record, value);
      }

      if (!changed)
        return null;

      if (!IsEntityValid(table, record, messages))
        return null;

      int? newId = SaveRecord(userId, record, table.DataSet, dataSession);

      Debug.Assert(newId.HasValue);

      return newId;

    }

    public virtual IDataSetRecord CreateRecord(Type type)
    {
      return (IDataSetRecord)Activator.CreateInstance(type);
    }

    public virtual IDataSetRecord GetRecord(ISessionToken userId, int id, DataSet dataSet, IDataSession dataSession)
    {
      var entity = BusinessServiceManager.GetService(dataSet.EntityType).GetById(userId, id, dataSession);

      return (IDataSetRecord)DataServices.Resolve<IEntityObjectMapper>().Map(entity, dataSet.Type);
    }

    public virtual int? SaveRecord(ISessionToken userId, IDataSetRecord record, DataSet dataSet, IDataSession dataSession)
    {
      var entity = (EntityObject)DataServices.Resolve<IEntityObjectMapper>().Map(record, dataSet.EntityType);

      return BusinessServiceManager.GetService(dataSet.EntityType).Save(userId, entity, dataSession);
    }

    private bool IsEntityValid(DataSetTable table, IDataSetRecord record, MessageCollection messages)
    {
      var errors = DataSetValidationVisitor.GetMessages(table.DataSet, record);

      foreach (var error in errors)
      {
        messages.AddError($"{error.Restriction.Column.Label}: {error.Message}", table.DataSet, record);
      }

      return errors.Count == 0;
    }
  }
}
