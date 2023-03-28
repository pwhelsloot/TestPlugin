using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server.SQL;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.BslTriggers.Data.SQL
{
  public class SQLBslTriggerServiceAccess : SQLEntityObjectAccess<BslTriggerEntity>, IBslTriggerServiceAccess
  {
    private const string Select = @"
          BslTriggerId,
          Description,
          TriggerEntity,
          TriggerOnCreate,
          TriggerOnUpdate,
          TriggerOnDelete,
          Action,
          ActionConfiguration,
          ActionGuid,
          UseJobSystem
        ";

    private const string From = @"BslTrigger";

    public IList<BslTriggerEntity> GetAllBySystemCategory(IDataSession dataSession, ISessionToken userId, string category)
    {
      var query = SQLQueryBuilder.FromCriteria(Criteria.For(typeof(BslTriggerEntity)).Add(Expression.Eq(nameof(BslTriggerEntity.SystemCategory), category)), CriteriaQueryType.Select);
      query.SetSelect(Select);
      query.AddSelect(@"SystemCategory");

      query.SetFrom(From);

      return dataSession.Query(query).Execute().List<BslTriggerEntity>();
    }

    public BslTriggerEntity GetByEntityActionGuidCategory(IDataSession dataSession, ISessionToken userId, string entity, Guid actionGuid, string category, string actionConfiguration)
    {
      var criteria = Criteria.For(typeof(BslTriggerEntity))
        .Add(Expression.Eq(nameof(BslTriggerEntity.TriggerEntity), entity))
        .Add(Expression.Eq(nameof(BslTriggerEntity.ActionGuid), actionGuid));

      criteria.Add(category != null 
        ? Expression.Eq(nameof(BslTriggerEntity.SystemCategory), category) 
        : Expression.Null(nameof(BslTriggerEntity.SystemCategory)));

      criteria.Add(actionConfiguration != null
        ? Expression.Eq(nameof(BslTriggerEntity.ActionConfiguration), actionConfiguration)
        : Expression.Null(nameof(BslTriggerEntity.ActionConfiguration)));

      var query = SQLQueryBuilder.FromCriteria(criteria, CriteriaQueryType.Select);
      query.SetSelect(Select);
      query.AddSelect(@"SystemCategory");

      query.SetFrom(From);

      return dataSession.Query(query).Execute().FirstOrDefault<BslTriggerEntity>();
    }
  }
}
