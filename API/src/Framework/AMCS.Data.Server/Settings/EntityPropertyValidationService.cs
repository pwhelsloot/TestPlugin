//-----------------------------------------------------------------------------
// <copyright file="EntityPropertyValidationService.cs" company="AMCS Group">
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
using AMCS.Data;
using AMCS.Data.Entity.Validation;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.Settings.Data;

namespace AMCS.Data.Server.Settings
{
  public class EntityPropertyValidationService : EntityObjectService<EntityPropertyValidationEntity>, IEntityPropertyValidationService
  {
    private const int DirectDebitEntityId = 2;

    private const string DirectDebitEntity = "AMCS.Elemos.Entity.DirectDebitEntity";

    public EntityPropertyValidationService(IEntityObjectAccess<EntityPropertyValidationEntity> dataAccess)
      : base(dataAccess)
    {
    }

    /// <summary>
    /// Returns all EntityPropertyValidationEntity for a specified ClassName and PropertyName
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="className">If not null, limits returned results to those that match provided ClassName</param>
    /// <param name="propertyName">If not null, limits returned results to those that match provided PropertyName</param>
    /// <param name="existingDataSession">Data Session to use. Will create new one if null.</param>
    /// <returns>Collection of EntityPropertyValidationEntity</returns>
    public IList<EntityPropertyValidationEntity> GetAllByClassNameAndPropertyName(string className = null, string propertyName = null, IDataSession existingDataSession = null)
    {
      IDataSession dataSession = GetDataSession(DataServices.Resolve<IUserService>().SystemUserSessionKey, existingDataSession);
      try
      {
        return ((IEntityPropertyValidationAccess)DataAccess).GetAllByClassNameAndPropertyName(dataSession, className, propertyName);
      }
      finally
      {
        DisposeDataSession(dataSession, existingDataSession);
      }
    }

    public IList<EntityPropertyValidationEntity> GetAllByValidationContext(ISessionToken userId, int validationContextId, string entity, IDataSession existingDataSession = null)
    {
      IDataSession dataSession = GetDataSession(userId, existingDataSession);
      try
      {
        var validations = ((IEntityPropertyValidationAccess)DataAccess).GetAllByValidationContext(dataSession, validationContextId);

        if (entity == DirectDebitEntity)
        {
          ApplyRestrictions(validations);
        }

        return validations;
      }
      finally
      {
        DisposeDataSession(dataSession, existingDataSession);
      }
    }

    /// <summary>
    /// Set restrictions on the property validations to limit settings sector configuration.
    /// </summary>
    /// <remarks>
    /// Should probably be moved to the database.
    /// </remarks>
    /// <param name="validations"></param>
    private void ApplyRestrictions(IList<EntityPropertyValidationEntity> validations)
    {
      // mark non-nullable fields "always mandatory"
      foreach (var validation in validations)
      {
        switch (validation.PropertyName)
        {
          case "DirectDebitRunConfigurationId":
          case "BankName":
          case "AccountName":
          case "IsVerified":
            {
              validation.AlwaysMandatory = true;
              validation.Mandatory = true;
              validation.Display = true;
            }
            break;

          // RDM - These haven't been implemented for EPR side, only for Portal so need to lock them down.
          case "Address1":
          case "Address2":
          case "Address3":
          case "Address4":
          case "Address5":
          case "PostCode":
            {
              validation.AlwaysMandatory = true;
              validation.Mandatory = false;
              validation.Display = true;
            }
            break;

          default:
            break;
        }

        // enable regex on string fields
        switch (validation.PropertyName)
        {
          case "BankName":
          case "AccountName":
          case "AccountNo":
          case "SortCode":
          case "NationalBankCode":
          case "NationalCheckDigits":
          case "RIBNumber":
          case "BIC":
          case "IBAN":
          case "AuthorisedSignatory":
          case "BranchCode":
            {
              validation.AllowRegexValidation = true;
            }
            break;

          default:
            break;
        }
      }
    }
  }
}