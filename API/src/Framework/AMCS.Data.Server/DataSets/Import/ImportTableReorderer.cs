using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportTableReorderer
  {
    private readonly DataSetTable table;
    private readonly MessageCollection messages;
    private readonly HashSet<object> seen = new HashSet<object>();
    private readonly HashSet<object> added = new HashSet<object>();
    private Dictionary<object, IDataSetRecord> idMap;
    private readonly EntityObjectProperty keyColumnProperty;
    private readonly List<DataSetColumn> relationships;
    private List<IDataSetRecord> result;

    public ImportTableReorderer(DataSetTable table, MessageCollection messages)
    {
      this.table = table;
      this.messages = messages;

      relationships = this.table.DataSet.Relationships
        .Where(p => p.To == this.table.DataSet)
        .Select(p => this.table.Columns.SingleOrDefault(p1 => p1 == p.FromColumn))
        .Where(p => p != null)
        .ToList();

      keyColumnProperty = this.table.DataSet.KeyColumn.Property;
    }

    public IList<IDataSetRecord> GetOrdered()
    {
      // If this table has internal foreign keys, we need to ensure that the records
      // are ordered correctly.

      if (relationships.Count == 0)
        return table.Records;

      idMap = new Dictionary<object, IDataSetRecord>();

      foreach (var importRecord in table.Records)
      {
        var key = keyColumnProperty.GetValue(importRecord);
        //Check if key is already been added for this entity
        if (idMap.TryGetValue(key, out _))
        {
          messages.AddError($"{table.DataSet.Name} already been contains {key}", table.DataSet, importRecord);
        }
        else
        {
          idMap.Add(key, importRecord);
        }
      }

      result = new List<IDataSetRecord>();

      foreach (var record in table.Records)
      {
        AddRecord(record);
      }

      return result;
    }

    private bool AddRecord(IDataSetRecord record)
    {
      object id = keyColumnProperty.GetValue(record);
      if (!seen.Add(id))
        return false;

      foreach (var relationship in relationships)
      {
        object relationshipId = relationship.Property.GetValue(record);
        if (relationshipId != null && !Equals(id, relationshipId) && !AddRelationshipRecord(relationshipId))
          messages.AddError($"Could not correctly order records because relationship '{relationship.Property.Name}' could not be added", table.DataSet, record);
      }

      added.Add(id);
      result.Add(record);

      return true;
    }

    private bool AddRelationshipRecord(object id)
    {
      // If we don't have this record, we assume it's referencing an existing record
      // in the database, so skip it.

      if (!idMap.TryGetValue(id, out var relationship))
        return true;

      // Succeed if we already have it.

      if (added.Contains(id))
        return true;

      // Otherwise, add it as usual.

      return AddRecord(relationship);
    }
  }
}
