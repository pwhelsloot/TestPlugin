//-----------------------------------------------------------------------------
// <copyright file="DataAccessObjectManager.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public static class DataAccessManager
  {
    /// <summary>
    /// Returns a newly constructed data access object that implements the interface T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetAccessForInterface<T>() where T : IEntityObjectAccess
    {
      return DataServices.Resolve<T>();
    }

    /// <summary>
    /// Gets the entity access for entity.  If there is a custom data access type for entity type T then return this, otheriwse
    /// return a default instance that can handle entity type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEntityObjectAccess<T> GetAccessForEntity<T>() where T : EntityObject
    {
      return DataServices.Resolve<IEntityObjectAccess<T>>();
    }

    /// <summary>
    /// Gets the entity access for entity.  If there is a custom data access type for entity type then return this, otheriwse
    /// return a default instance that can handle entity type.
    /// </summary>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <returns></returns>
    public static IEntityObjectAccess GetAccessForEntity(Type entityObjectType)
    {
      return (IEntityObjectAccess)DataServices.Resolve(typeof(IEntityObjectAccess<>).MakeGenericType(entityObjectType));
    }

    /// <summary>
    /// Gets the search access for interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetAccessForSearchInterface<T>() where T : ISearchDataAccessProvider
    {
      return DataServices.Resolve<T>();
    }
  }
}