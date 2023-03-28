using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Expression = System.Linq.Expressions.Expression;

namespace AMCS.Data.TestSupport
{
  public static class MockExtensions
  {
    public static void SetupGetNew<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, TEntity entity)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(entity);
      var nonGeneric = ConvertGetNew(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(entity);
    }

    public static void SetupGetNewAsCopy<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, TEntity entity)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(entity);
      var nonGeneric = ConvertGetNewAsCopy(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(entity);
    }

    public static void SetupGetById<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, TEntity entity)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(entity);
      var nonGeneric = ConvertGetById(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(entity);
    }

    public static void SetupGetByGuid<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, TEntity entity)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(entity);
      var nonGeneric = ConvertGetByGuid(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(entity);
    }

    public static void SetupGetAllById<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, IList<TEntity> results)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(results);
      var nonGeneric = ConvertGetAllById(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(results.Cast<EntityObject>().ToList());
    }

    public static void SetupGetAllByParentId<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, IList<TEntity> results)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(results);
      var nonGeneric = ConvertGetAllByParentId(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(results.Cast<EntityObject>().ToList());
    }

    public static void SetupGetAllByTemplate<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, IList<TEntity> results)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(results);
      var nonGeneric = ConvertGetAllByTemplate(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(results.Cast<EntityObject>().ToList());
    }

    public static void SetupSave<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, int?>> expression, int? id)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(id);
      var nonGeneric = ConvertSave(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(id);
    }

    public static void SetupDelete<TService, TEntity>(this Mock<TService> self, Expression<Action<TService>> expression)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression);
      var nonGeneric = ConvertDelete(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric);
    }

    public static void SetupDeleteAllByTemplate<TService, TEntity>(this Mock<TService> self, Expression<Action<TService>> expression)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression);
      var nonGeneric = ConvertDeleteAllByTemplate(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric);
    }

    public static void SetupUnDelete<TService, TEntity>(this Mock<TService> self, Expression<Action<TService>> expression)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression);
      var nonGeneric = ConvertUnDelete(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric);
    }

    public static void SetupGetAllByCriteria<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, IList<TEntity> results)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      self.Setup(expression).Returns(results);
      var nonGeneric = ConvertGetAllByCriteria(expression);
      self.As<IEntityObjectService>().Setup(nonGeneric).Returns(results.Cast<EntityObject>().ToList());
    }

    public static void VerifyGetNew<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, TEntity>(self, expression, times))
      {
        return;
      }
      
      var nonGenericExpression = ConvertGetNew(expression);
      self.As<IEntityObjectService>().Verify(nonGenericExpression, times);
    }

    public static void VerifyGetNewAsCopy<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, Times times)
    where TService : class, IEntityObjectService<TEntity>
    where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, TEntity>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetNewAsCopy(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyGetById<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, TEntity>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetById(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyGetByGuid<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, TEntity>> expression, Times times)
        where TService : class, IEntityObjectService<TEntity>
        where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, TEntity>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetByGuid(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyGetAllById<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, IList<TEntity>>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetAllById(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyGetAllByParentId<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, IList<TEntity>>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetAllByParentId(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyGetAllByTemplate<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, IList<TEntity>>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetAllByTemplate(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifySave<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, int?>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, int?>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertSave(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyDelete<TService, TEntity>(this Mock<TService> self, Expression<Action<TService>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertDelete(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyDeleteAllByTemplate<TService, TEntity>(this Mock<TService> self, Expression<Action<TService>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertDeleteAllByTemplate(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyUnDelete<TService, TEntity>(this Mock<TService> self, Expression<Action<TService>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertUnDelete(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    public static void VerifyGetAllByCriteria<TService, TEntity>(this Mock<TService> self, Expression<Func<TService, IList<TEntity>>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      if (VerifyGeneric<TService, TEntity, IList<TEntity>>(self, expression, times))
      {
        return;
      }

      var nonGeneric = ConvertGetAllByCriteria(expression);
      self.As<IEntityObjectService>().Verify(nonGeneric, times);
    }

    private static Expression<Func<IEntityObjectService, EntityObject>> ConvertGetNew<TService, TEntity>(Expression<Func<TService, TEntity>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");

      MethodInfo methodInfo = null;

      if (call.Arguments.Count == 2)
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("GetNew", new Type[] { typeof(ISessionToken), typeof(IDataSession) });
      }
      else if (call.Arguments.Count > 2)
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("GetNew", new Type[] { typeof(ISessionToken), typeof(IDataSession), typeof(object[]) });
      }

      Expression nonGenericCall = Expression.Call(
        parameter,
        methodInfo,
        call.Arguments);

      return Expression.Lambda<Func<IEntityObjectService, EntityObject>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, EntityObject>> ConvertGetNewAsCopy<TService, TEntity>(Expression<Func<TService, TEntity>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("GetNewAsCopy", new Type[] { typeof(ISessionToken), typeof(EntityObject) }),
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, EntityObject>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, EntityObject>> ConvertGetById<TService, TEntity>(Expression<Func<TService, TEntity>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("GetById", new Type[] { typeof(ISessionToken), typeof(int), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, EntityObject>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, EntityObject>> ConvertGetByGuid<TService, TEntity>(Expression<Func<TService, TEntity>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("GetByGuid", new Type[] { typeof(ISessionToken), typeof(Guid), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, EntityObject>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, IList<EntityObject>>> ConvertGetAllById<TService, TEntity>(Expression<Func<TService, IList<TEntity>>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("GetAllById", new Type[] { typeof(ISessionToken), typeof(int), typeof(bool), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, IList<EntityObject>>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, IList<EntityObject>>> ConvertGetAllByParentId<TService, TEntity>(Expression<Func<TService, IList<TEntity>>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("GetAllByParentId", new Type[] { typeof(ISessionToken), typeof(int), typeof(bool), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, IList<EntityObject>>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, IList<EntityObject>>> ConvertGetAllByTemplate<TService, TEntity>(Expression<Func<TService, IList<TEntity>>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");

      MethodInfo methodInfo = null;
      if (call.Arguments.Count == 4)
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("GetAllByTemplate", new Type[] { typeof(ISessionToken), typeof(TEntity), typeof(bool), typeof(IDataSession) });
      }
      else if (call.Arguments.Count == 5)
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("GetAllByTemplate", new Type[] { typeof(ISessionToken), typeof(TEntity), typeof(bool), typeof(IList<string>), typeof(IDataSession) });
      }
      else if (call.Arguments.Count == 6)
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("GetAllByTemplate", new Type[] { typeof(ISessionToken), typeof(TEntity), typeof(bool), typeof(IList<string>), typeof(IList<string>), typeof(IDataSession) });
      }

      Expression nonGenericCall = Expression.Call(
        parameter,
        methodInfo,
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, IList<EntityObject>>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, int?>> ConvertSave<TService>(Expression<Func<TService, int?>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");

      MethodInfo methodInfo = null;
      if (typeof(EntityObject).IsAssignableFrom(call.Arguments[1].Type))
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("Save", new Type[] { typeof(ISessionToken), typeof(EntityObject), typeof(IDataSession) });
      }
      else
      {
        methodInfo = typeof(IEntityObjectService).GetMethod("Save", new Type[] { typeof(ISessionToken), typeof(IList<EntityObject>), typeof(IDataSession) });
      }

      Expression nonGenericCall = Expression.Call(
        parameter,
        methodInfo,
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, int?>>(nonGenericCall, parameter);
    }

    private static Expression<Action<IEntityObjectService>> ConvertDelete<TService>(Expression<Action<TService>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("Delete", new Type[] { typeof(ISessionToken), typeof(EntityObject), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Action<IEntityObjectService>>(nonGenericCall, parameter);
    }

    private static Expression<Action<IEntityObjectService>> ConvertDeleteAllByTemplate<TService>(Expression<Action<TService>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("DeleteAllByTemplate", new Type[] { typeof(ISessionToken), typeof(EntityObject), typeof(bool), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Action<IEntityObjectService>>(nonGenericCall, parameter);
    }

    private static Expression<Action<IEntityObjectService>> ConvertUnDelete<TService>(Expression<Action<TService>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("UnDelete", new Type[] { typeof(ISessionToken), typeof(EntityObject), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Action<IEntityObjectService>>(nonGenericCall, parameter);
    }

    private static Expression<Func<IEntityObjectService, IList<EntityObject>>> ConvertGetAllByCriteria<TService, TEntity>(Expression<Func<TService, IList<TEntity>>> expression)
    {
      MethodCallExpression call = expression.Body as MethodCallExpression;
      ParameterExpression parameter = Expression.Parameter(typeof(IEntityObjectService), "service");
      Expression nonGenericCall = Expression.Call(
        parameter,
        typeof(IEntityObjectService).GetMethod("GetAllByCriteria", new Type[] { typeof(ISessionToken), typeof(ICriteria), typeof(IDataSession) }),
        call.Arguments);
      return Expression.Lambda<Func<IEntityObjectService, IList<EntityObject>>>(nonGenericCall, parameter);
    }

    private static bool VerifyGeneric<TService, TEntity, TReturn>(Mock<TService> mock, Expression<Func<TService, TReturn>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      try
      {
        mock.Verify(expression, times);
        return true;
      }
      catch (MockException)
      {
        // this means the verification has failed on the generic method setup.
        // suppress this exception so the caller can try the Verify on the non-generic method setup.
      }

      return false;
    }

    private static bool VerifyGeneric<TService, TEntity>(Mock<TService> mock, Expression<Action<TService>> expression, Times times)
      where TService : class, IEntityObjectService<TEntity>
      where TEntity : EntityObject
    {
      try
      {
        mock.Verify(expression, times);
        return true;
      }
      catch (MockException)
      {
        // this means the verification has failed on the generic method setup.
        // suppress this exception so the caller can try the Verify on the non-generic method setup.
      }

      return false;
    }
  }
}
