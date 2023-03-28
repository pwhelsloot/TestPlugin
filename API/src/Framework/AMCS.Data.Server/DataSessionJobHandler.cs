using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem;

namespace AMCS.Data.Server
{
  public abstract class DataSessionJobHandler<TRequest> : JobHandler<TRequest>
  {
    protected override void Execute(IJobContext context, ISessionToken userId, TRequest request)
    {
      using (IDataSession session = BslDataSessionFactory.GetDataSession(userId))
      {
        try
        {
          session.StartTransaction();

          Execute(context, userId, request, session);

          session.CommitTransaction();
        }
        catch
        {
          session.RollbackTransaction();
          throw;
        }
      }
    }

    protected abstract void Execute(IJobContext context, ISessionToken userId, TRequest request, IDataSession session);
  }

  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Same class with different generic parameters")]
  public abstract class DataSessionJobHandler<TRequest, TResponse> : JobHandler<TRequest, TResponse>
  {
    protected override TResponse Execute(IJobContext context, ISessionToken userId, TRequest request)
    {
      using (IDataSession session = BslDataSessionFactory.GetDataSession(userId))
      {
        try
        {
          session.StartTransaction();

          var result = Execute(context, userId, request, session);

          session.CommitTransaction();

          return result;
        }
        catch
        {
          session.RollbackTransaction();
          throw;
        }
      }
    }

    protected abstract TResponse Execute(IJobContext context, ISessionToken userId, TRequest request, IDataSession session);
  }
}
