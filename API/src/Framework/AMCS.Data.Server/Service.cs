#region Header

////-----------------------------------------------------------------------------
//// <copyright file="Service.cs" company="AMCS Group">
////   Copyright © 2010-12 AMCS Group. All rights reserved.
//// </copyright>
////
//// PROJECT: P142 - Elemos
////
//// AMCS Elemos Project
////
////-----------------------------------------------------------------------------

#endregion Header

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server.SQL;

namespace AMCS.Data.Server
{
  /// <summary>
  /// Service class.
  /// </summary>
  public abstract class Service : IService
  {
    #region Properties/Variables

    /// <summary>
    /// Gets or sets a value indicating whether [use reporting database].
    /// </summary>
    /// <value>
    /// <c>true</c> if [use reporting database]; otherwise, <c>false</c>.
    /// </value>
    public bool UseReportingDatabase { get; set; }

    #endregion Properties/Variables

    #region Methods

    /// <summary>
    /// Disposes the data session.
    /// </summary>
    /// <param name="dataSessionInUse">The data session in use.</param>
    /// <param name="existingDataSessionPassedIn">The existing data session passed in.</param>
    protected void DisposeDataSession(IDataSession dataSessionInUse, IDataSession existingDataSessionPassedIn)
    {
      DisposeDataSession(dataSessionInUse, existingDataSessionPassedIn != null);
    }

    /// <summary>
    /// Disposes the data session.
    /// </summary>
    /// <param name="dataSessionInUse">The data session in use.</param>
    /// <param name="isUsingExistingDataSession">if set to <c>true</c> [is using existing data session].</param>
    protected void DisposeDataSession(IDataSession dataSessionInUse, bool isUsingExistingDataSession)
    {
      if (!isUsingExistingDataSession && dataSessionInUse != null)
        dataSessionInUse.Dispose();
    }

    /// <summary>
    /// Gets the data session.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    protected IDataSession GetDataSession(ISessionToken userId, IDataSession existingDataSession, bool? isDetached = false)
    {
      if (existingDataSession != null)
        return existingDataSession;

      if (StrictMode.IsRequireTransaction)
        throw new StrictModeException("Data session must be provided when strict mode is enabled");

      return BslDataSessionFactory.GetDataSession(userId, this.UseReportingDatabase, isDetached);
    }

    /// <summary>
    /// Get the integer identifier from guid.
    /// </summary>
    /// <param name="dataSession">The dataSession.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <returns></returns>
    public int? GetIntIdFromGUID(IDataSession dataSession, ISessionToken userId, Type entityType, Guid? guid)
    {
      return dataSession.GetIntIdFromGUID(userId, entityType, guid);
    }

    #endregion Methods
  }
}