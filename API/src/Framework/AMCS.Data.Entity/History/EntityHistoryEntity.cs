
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Newtonsoft.Json;

namespace AMCS.Data.Entity.History
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class EntityHistory : EntityObject
  {
    private int? _EntityHistoryId;
    [DataMember(Name = "EntityHistoryId")]
    public int? EntityHistoryId
    {
      get { return _EntityHistoryId; }
      set { if (_EntityHistoryId != value) { _EntityHistoryId = value; NotifyChange(() => EntityHistoryId); } }
    }

    private string _Table;
    [DataMember(Name = "Table")]
    public string Table
    {
      get { return _Table; }
      set { if (_Table != value) { _Table = value; NotifyChange(() => Table); } }
    }

    private int? _TableId;
    [DataMember(Name = "TableId")]
    public int? TableId
    {
      get { return _TableId; }
      set { if (_TableId != value) { _TableId = value; NotifyChange(() => TableId); } }
    }

    private ZonedDateTime? _Date;
    [EntityMember(DateStorage = DateStorage.Neutral)]
    [DataMember(Name = "Date")]
    public ZonedDateTime? Date
    {
      get { return _Date; }
      set { if (_Date != value) { _Date = value; NotifyChange(() => Date); } }
    }

    private int? _SysUserId;
    [DataMember(Name = "SysUserId")]
    public int? SysUserId
    {
      get { return _SysUserId; }
      set { if (_SysUserId != value) { _SysUserId = value; NotifyChange(() => SysUserId); } }
    }

    private string _Changes;
    [DataMember(Name = "Changes")]
    public string Changes
    {
      get { return _Changes; }
      set { if (_Changes != value) { _Changes = value; NotifyChange(() => Changes); } }
    }

    private int? _EntityHistoryTypeId;
    [DataMember(Name = "EntityHistoryTypeId")]
    public int? EntityHistoryTypeId
    {
      get { return _EntityHistoryTypeId; }
      set { if (_EntityHistoryTypeId != value) { _EntityHistoryTypeId = value; NotifyChange(() => EntityHistoryTypeId); } }
    }

    private string _SysUser;
    [DynamicColumn("SysUser")]
    [DataMember(Name = "SysUser")]
    public string SysUser
    {
      get { return _SysUser; }
      set { if (_SysUser != value) { _SysUser = value; NotifyChange(() => SysUser); } }
    }

    private IList<EntityHistoryChange> _TypedChanges;
    [DynamicColumn("TypedChanges")]
    [DataMember(Name = "TypedChanges")]
    public IList<EntityHistoryChange> TypedChanges
    {
      get { return _TypedChanges; }
      set { if (_TypedChanges != value) { _TypedChanges = value; NotifyChange(() => TypedChanges); } }
    }

    private byte[] rowVersion;
    [DynamicColumn("RowVersion")]
    [DataMember(Name = "RowVersion")]
    public byte[] RowVersion
    {
      get { return rowVersion; }
      set {if (rowVersion != value) { rowVersion = value; NotifyChange(() => RowVersion); } }
    }

    private string _ParentTable;
    [DataMember(Name = "ParentTable")]
    public string ParentTable
    {
      get { return _ParentTable; }
      set { if (_ParentTable != value) { _ParentTable = value; NotifyChange(() => ParentTable); } }
    }

    private int? _ParentTableId;
    [DataMember(Name = "ParentTableId")]
    public int? ParentTableId
    {
      get { return _ParentTableId; }
      set { if (_ParentTableId != value) { _ParentTableId = value; NotifyChange(() => ParentTableId); } }
    }

    private string _CorrelationId;
    [DataMember(Name = "CorrelationId")]
    public string CorrelationId
    {
      get { return _CorrelationId; }
      set { if (_CorrelationId != value) { _CorrelationId = value; NotifyChange(() => CorrelationId); } }
    }

    public override string GetTableName()
    {
      return "EntityHistory";
    }

    public override string GetKeyName()
    {
      return "EntityHistoryId";
    }

    public override int? GetId()
    {
      return this.EntityHistoryId;
    }
  }
}
