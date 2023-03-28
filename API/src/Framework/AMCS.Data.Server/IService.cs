//-----------------------------------------------------------------------------
// <copyright file="IService.cs" company="AMCS Group">
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
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;

namespace AMCS.Data.Server
{
  /// <summary>
  /// The Service Interface.
  /// </summary>
  public interface IService
  {
    /// <summary>
    /// Gets or sets a value indicating whether [use reporting database].
    /// </summary>
    /// <value>
    /// <c>true</c> if [use reporting database]; otherwise, <c>false</c>.
    /// </value>
    bool UseReportingDatabase { get; set; }

    /// <summary>
    /// Gets the integer identifier from unique identifier.
    /// </summary>
    /// <param name="dataSession">The data session.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <returns></returns>
    int? GetIntIdFromGUID(IDataSession dataSession, ISessionToken userId, Type entityType, Guid? guid);
  }
}