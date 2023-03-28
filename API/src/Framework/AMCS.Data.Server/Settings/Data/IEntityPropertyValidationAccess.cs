//-----------------------------------------------------------------------------
// <copyright file="IEntityPropertyValidationAccess.cs" company="AMCS Group">
//   Copyright © 2010-14 AMCS Group. All rights reserved.
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
using AMCS.Data.Entity.Validation;

namespace AMCS.Data.Server.Settings.Data
{
  public interface IEntityPropertyValidationAccess : IEntityObjectAccess<EntityPropertyValidationEntity>
  {
    /// <summary>
    /// Returns all EntityPropertyValidationEntity for a specified ClassName and PropertyName
    /// </summary>
    /// <param name="dataSession">Data Session</param>
    /// <param name="className">If not null, limits returned results to those that match provided ClassName</param>
    /// <param name="propertyName">If not null, limits returned results to those that match provided PropertyName</param>
    /// <returns>Collection of EntityPropertyValidationEntity</returns>
    IList<EntityPropertyValidationEntity> GetAllByClassNameAndPropertyName(IDataSession dataSession, string className = null, string propertyName = null);

    /// <summary>
    /// Returns all entity property validations for the specified validation context.
    /// </summary>
    IList<EntityPropertyValidationEntity> GetAllByValidationContext(IDataSession dataSession, int validationContextId);
  }
}