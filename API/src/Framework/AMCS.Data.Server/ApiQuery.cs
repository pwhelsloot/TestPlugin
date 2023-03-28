using System.Collections.Generic;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public class ApiQuery
  {
    public static readonly ApiQuery Empty = new ApiQuery(new EntityObject[0], 0);

    public static ApiQuery Single(EntityObject entityObject)
    {
      return new ApiQuery(new[] { entityObject }, 1);
    }

    public IList<EntityObject> Entities { get; }
    public int? Count { get; }
    public EntityObject Summary { get; }
    
    public ApiQuery(IList<EntityObject> entities, int? count)
    {
      Entities = entities;
      Count = count;
    }

    public ApiQuery(IList<EntityObject> entities, int? count, EntityObject summary) 
        : this(entities, count)
    {
      Summary = summary;
    }
  }

  public class ApiQuery<T> : ApiQuery
    where T : EntityObject
  {
    public new IList<T> Entities { get; }

    public ApiQuery(IList<EntityObject> entities, int? count)
      : this(entities, count, null)
    {

    }

    public ApiQuery(IList<EntityObject> entities, int? count, EntityObject summary) 
      : base(entities, count, summary)
    {
      Entities = new List<T>(entities.Count);

      foreach (var entity in entities)
      { 
        Entities.Add((T)entity);
      }
    }
  }
}
