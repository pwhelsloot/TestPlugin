using AMCS.Data;
using AMCS.Data.EndToEnd.Tests;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace AMCS.Data.EndToEnd.Tests
{
  public static class ScenarioContextExtentions
  {
    private static IProfileBuilderManager profileBuilderManager = DataServices.Resolve<IProfileBuilderManager>();

    public static TEntity GetNamedEntityIfNameNotNullOrEmpty<TEntity, TBuilder>(this ScenarioContext context, string name)
      where TEntity : EntityObject
      where TBuilder : BaseBuilder
    {
      if (!String.IsNullOrEmpty(name))
      {
        return context.GetNamedEntity<TEntity, TBuilder>(name);
      }

      return null;
    }

    public static TEntity GetNamedEntity<TEntity, TBuilder>(this ScenarioContext context, string name)
      where TEntity : EntityObject
      where TBuilder : BaseBuilder
    {
      if (context.TryGetValue<TEntity>(typeof(TEntity).FullName + ":" + name, out var result))
        return result;

      TBuilder builder = profileBuilderManager.GetProfileEntityByName<TEntity, TBuilder> (name, context);
      if (builder != null)
      {
        var dataSession = context.Get<IDataSession>();
        var adminUserId = context.Get<ISessionToken>();
        TEntity buildResult = (TEntity)builder.Build(adminUserId, dataSession);
        if (buildResult != null)
        {
          context.SetNamedEntity<TEntity>(name, buildResult);
          return buildResult;
        }
      }

      throw new InvalidOperationException();
    }

    public static void AddNamedEntity<T>(this ScenarioContext context, string name, T entity)
      where T : EntityObject
    {
      context.Add(typeof(T).FullName + ":" + name, entity);
    }

    public static void SetNamedEntity<T>(this ScenarioContext context, string name, T entity)
      where T : EntityObject
    {
      context[typeof(T).FullName + ":" + name] = entity;
    }

    public static bool ContainsNamedEntity<T>(this ScenarioContext context, string name)
      where T : EntityObject
    {
      return context.ContainsKey(typeof(T).FullName + ":" + name);
    }
  }
}
