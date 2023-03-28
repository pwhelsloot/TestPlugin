using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Class)]
  public class EntityTableAttribute : Attribute
  {
    public string TableName { get; }

    public string KeyField { get; }

    public string SchemaName { get; set; }

    public string ObjectName { get; set; }

    public IdentityInsertMode IdentityInsertMode { get; set; }

    public bool TrackInserts { get; set; }
    public bool TrackUpdates { get; set; }
    public bool TrackDeletes { get; set; }


    /// <summary>
    /// If set to true, an id will only be generated for this entity on insert if it's null
    /// </summary>
    public bool InsertOnNullId { get; set; }
    
    public EntityTableAttribute(string tableName, string keyField)
    {
      TableName = tableName;
      KeyField = keyField;
      SchemaName = "dbo";
      TrackInserts = false;
      TrackUpdates = false;
      TrackDeletes = false;
    }

    public EntityTableAttribute(string tableName, string keyField, string objectName)
    {
      TableName = tableName;
      KeyField = keyField;
      ObjectName = objectName;
      SchemaName = "dbo";
      TrackInserts = false;
      TrackUpdates = false;
      TrackDeletes = false;
    }


    public EntityTableAttribute()
    {
    }
  }
}
