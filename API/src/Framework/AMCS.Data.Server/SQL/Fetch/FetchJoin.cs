#pragma warning disable 0618

using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Fetch
{
  /// <summary>
  /// Stores join details of a single table (including the first table in the From clause). Thic includes:
  /// - How to create the select and join part of the query.
  /// - How to read data from a DataReader into this entity.
  /// - How this table is joined to previous tables in the From clause.
  /// - How to assign the Parent/Child references between this table and its linked join parent table.
  /// </summary>
  internal class FetchJoin
  {
    private readonly EntityObjectAccessor accessor;
    private readonly int index;
    private readonly string prefix;
    private readonly string keyName;
    private readonly IEntityObjectReference referenceAssigner;
    // To avoid cartesian explosion in the joins, some queries needs to be split up using union all
    // When we do this, a given join might only be included in some of these sub queries, though it still need to have the same columns in select.
    // We identify each sub query as a group with index 0, 1, ..., N. This collection tracks whether this query is active for a given such group.
    private readonly List<GroupJoinState> groupJoinState = new List<GroupJoinState>();

    public int? JoinParentIndex { get; }
    public Type Type => accessor.Type;    

    private FetchJoin(EntityObjectAccessor accessor, int index, int? joinParentIndex, IEntityObjectReference referenceAssigner)
    {
      this.JoinParentIndex = joinParentIndex;
      this.accessor = accessor;
      this.index = index;
      this.prefix = $"j{index}_";
      this.keyName = prefix + accessor.KeyName;
      this.referenceAssigner = referenceAssigner;
    }

    public FetchJoin(EntityObjectAccessor accessor, int groupCount)
      : this(accessor, index: 0, joinParentIndex: null, referenceAssigner: null)
    {
      // The main table must be included in all groups (aka sub queries)
      groupJoinState = Enumerable
        .Repeat(GroupJoinState.InnerJoin, groupCount)
        .ToList();
    }

    public FetchJoin(int index, int joinParentIndex, IEntityObjectReference referenceAssigner)
      : this(referenceAssigner.TargetAccessor, index, joinParentIndex, referenceAssigner)
    { }

    public void WriteSelectFields(SQLTextBuilder sql, int groupIndex)
    {
      bool isActive = GetJoinStateForGroup(groupIndex) != GroupJoinState.Excluded;

      bool isFirst = (referenceAssigner == null);
      var columnNames = accessor.Properties
        .Where(property => property.Column != null && !property.IsDynamicColumn)
        .Select(property => property.Column.ColumnName);
      foreach (var columnName in columnNames)
      {
        if (!isFirst)
        {
          sql
            .Text(",")
            .Text(Environment.NewLine)
            .Text("  ");
        }
        isFirst = false;

        if (isActive)
        {
          sql
            .Name($"j{index}")
            .Text(".")
            .Name(columnName);
        }
        else
        {
          sql.Text("NULL");
        }
        sql
          .Text(" ")
          .Name($"j{index}_{columnName}");
      }
    }

    public void WriteFromJoin(SQLTextBuilder sql, int groupIndex)
    {
      var joinState = GetJoinStateForGroup(groupIndex);

      if (joinState == GroupJoinState.Excluded)
        return;

      if (referenceAssigner == null)
      {
        sql
          .TableName(accessor)
          .Text(" ")
          .Name($"j{index}");
      }
      else
      {
        bool isLeftJoin = (joinState == GroupJoinState.LeftJoin);

        sql
          .Text(Environment.NewLine)
          .Text("  ")
          .Text(isLeftJoin ? "LEFT JOIN " : "JOIN ")
          .TableName(accessor)
          .Text(" ")
          .Name($"j{index}")
          .Text(" ON ")
          .Name($"j{index}")
          .Text(".")
          .Name(referenceAssigner.TargetColumnName)
          .Text(" = ")
          .Name($"j{JoinParentIndex.Value}")
          .Text(".")
          .Name(referenceAssigner.MainColumnName);
      }
    }

    public void Assign(EntityObject joinParentEntity, EntityObject joinChildEntity)
    {
      referenceAssigner.Assign(joinParentEntity, joinChildEntity);
    }

    public int? ReadId(IDataReader reader)
    {
      object value = reader[keyName];
      if (value is DBNull)
        return null;
      return (int)value;
    }

    public EntityObject ReadEntity(int id, IDataReader reader)
    {
      var entityObject = (EntityObject)DataRowToObjectConvertor.DoConvertDataRowToDataContractObject(reader, accessor, false, prefix);
      this.accessor.ResetReferences(entityObject);
      return entityObject;
    }

    public void SetJoinStateForGroup(int groupIndex, GroupJoinState joinState)
    {
      int missingEntries = groupIndex + 1 - groupJoinState.Count;
      if (missingEntries > 0)
        groupJoinState.AddRange(Enumerable.Repeat(GroupJoinState.Excluded, missingEntries));
      groupJoinState[groupIndex] = joinState;
    }

    private GroupJoinState GetJoinStateForGroup(int groupIndex)
    {
      if (groupJoinState.Count <= groupIndex)
        return GroupJoinState.Excluded;
      return groupJoinState[groupIndex];
    }
  }
}
