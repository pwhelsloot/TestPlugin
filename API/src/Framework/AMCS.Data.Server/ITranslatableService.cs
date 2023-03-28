//-----------------------------------------------------------------------------
// <copyright file="ITranslatableService.cs" company="AMCS Group">
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
using AMCS.Data.Configuration.Mapping.Translate;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  /// <summary>
  /// The Translatable Service Interface.
  /// </summary>
  public interface ITranslatableService : IService
  {
    /// <summary>
    /// Gets the entity object translator.
    /// </summary>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <returns></returns>
    EntityObjectTranslator GetEntityObjectTranslator(Type entityObjectType);

    /// <summary>
    /// Gets the translator.
    /// </summary>
    /// <param name="businessStringsType">Type of the business strings.</param>
    /// <returns></returns>
    BusinessObjectStringTranslator GetTranslator(Type businessStringsType);

    /// <summary>
    /// Validates the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="innerException">The inner exception.</param>
    void ValidateEntity(EntityObject entity, Exception innerException = null);
  }
}