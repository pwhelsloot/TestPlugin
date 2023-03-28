using AMCS.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Fetch
{
  /// <summary>
  /// This class reads the Entities from the Data Reader, when Fetch is in play.
  /// It reads both the main entity and all related entities and links them properly together.
  /// The class uses states, so Read should not be called more than once.
  /// </summary>
  internal class FetchReader
  {
    // Unchanged data
    private readonly IList<FetchJoin> joins;
    // State data
    private readonly IList results;
    private readonly Dictionary<(Type Type, int Id), EntityObject> entities = new Dictionary<(Type Type, int Id), EntityObject>();
    private readonly HashSet<(EntityObject Parent, EntityObject Child)> seen =
      new HashSet<(EntityObject, EntityObject)>(TupleByReferenceComparer<EntityObject, EntityObject>.Instance);
    private readonly HashSet<int> resultIdsSeen = new HashSet<int>();

    public FetchReader(FetchInfo fetchInfo, IList results)
    {
      this.joins = fetchInfo.Joins;
      this.results = results;
    }

    public void Read(IDataReader reader)
    {
      while (reader.Read())
      {
        ReadRow(reader);
      }
    }

    private void ReadRow(IDataReader reader)
    {
      var rowEntities = new EntityObject[joins.Count];

      for (int i = 0; i < joins.Count; i++)
      {
        var join = joins[i];

        var entity = ReadEntity(join, reader);
        rowEntities[i] = entity;

        if (entity == null)
          continue;

        // First column (primary entity)
        if (i == 0)
        {
          if (resultIdsSeen.Add(entity.Id32))
            results.Add(entity);
          continue;
        }

        var joinParentEntity = rowEntities[join.JoinParentIndex.Value];

        if (seen.Add((joinParentEntity, entity)))
          join.Assign(joinParentEntity, entity);
      }
    }

    private EntityObject ReadEntity(FetchJoin join, IDataReader reader)
    {
      int? id = join.ReadId(reader);

      if (!id.HasValue)
        return null;

      var key = (join.Type, (int)id);
      if (!entities.TryGetValue(key, out var entity))
      {
        entity = join.ReadEntity((int)id, reader);
        entities.Add(key, entity);
      }
      return entity;
    }
  }
}
