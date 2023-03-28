// //-----------------------------------------------------------------------------
// // <copyright file="BslDataSessionFactory.cs" company="AMCS Group">
// //   Copyright © 2010-12 AMCS Group. All rights reserved.
// // </copyright>
// //
// // PROJECT: P142 - Elemos
// //
// // AMCS Elemos Project
// //
// //-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server.Services;

namespace AMCS.Data.Server
{
  /// <summary>
  /// Data Session Factory.
  /// </summary>
  public static class BslDataSessionFactory
  {
    /// <summary>
    /// Start a new data session
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>
    /// Data session as configured by config file
    /// </returns>
    public static IDataSession GetDataSession(ISessionToken userId, bool? isDetached = false)
    {
      return GetDataSession(userId, false, isDetached);
    }

    /// <summary>
    ///   Start a new data session
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="useReportingDatabase">if set to <c>true</c> [use reporting database].</param>
    /// <returns>Data session as configured by config file</returns>
    public static IDataSession GetDataSession(ISessionToken userId, bool useReportingDatabase, bool? isDetached = false)
    {
      return DataServices.Resolve<IDataSessionFactory>().GetDataSession(userId, useReportingDatabase, isDetached);
    }

    /// <summary>
    ///   Start a new data session without checking the user session.
    /// </summary>
    /// <param name="useReportingDatabase">if set to <c>true</c> [use reporting database].</param>
    /// <returns>Data session as configured by config file</returns>
    public static IDataSession GetDataSession(bool useReportingDatabase = false, bool? isDetached = false)
    {
      return DataServices.Resolve<IDataSessionFactory>().GetDataSession(useReportingDatabase, isDetached);
    }
  }
}