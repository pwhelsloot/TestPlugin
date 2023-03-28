#region Header

////-----------------------------------------------------------------------------
//// <copyright file="TranslatableService.cs" company="AMCS Group">
////   Copyright © 2010-12 AMCS Group. All rights reserved.
//// </copyright>
////
//// PROJECT: P142 - Elemos
////
//// AMCS Elemos Project
////
////-----------------------------------------------------------------------------

#endregion Header

namespace AMCS.Data.Server
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using AMCS.Data.Configuration.Mapping.Translate;
  using AMCS.Data.Entity;
  using Data.Util.Extension;

  /// <summary>
  /// Translatable service.
  /// </summary>
  public abstract class TranslatableService : Service, ITranslatableService
  {
    #region Enumerations

    /// <summary>
    /// Service Base Strings.
    /// </summary>
    public enum BslServiceBaseStrings
    {
      [StringValue("Validation failed for the following reasons:\r\n{0}")]
      EntityValidationError,
    }

    #endregion Enumerations

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableService"/> class.
    /// </summary>
    public TranslatableService()
    {
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets the entity object translator.
    /// </summary>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <returns></returns>
    public EntityObjectTranslator GetEntityObjectTranslator(Type entityObjectType)
    {
      return new EntityObjectTranslator(entityObjectType.FullName, DataServices.Resolve<IMappingManagerAccessor>().EntityObjectMappingManager);
    }

    /// <summary>
    /// Gets the translator.
    /// </summary>
    /// <param name="businessStringsType">Type of the business strings.</param>
    /// <returns></returns>
    public BusinessObjectStringTranslator GetTranslator(Type businessStringsType)
    {
      return new BusinessObjectStringTranslator(this.GetType().FullName, businessStringsType);
    }

    /// <summary>
    /// Validates the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="innerException">The inner exception.</param>
    public void ValidateEntity(EntityObject entity, Exception innerException = null)
    {
      if (entity.RequiredFieldsEntered(out var errors))
        return;

      var sb = new StringBuilder();
      var validationErrors = new List<BslValidationError>();
      var translator = GetEntityObjectTranslator(entity.GetType());

      foreach (var error in errors)
      {
        string errorText;
        try
        {
          errorText = translator.GetLocalisedString(error.Value);
        }
        catch
        {
          errorText = null;
        }

        if (string.IsNullOrEmpty(errorText))
          errorText = error.Value;

        sb.AppendLine(errorText);
        validationErrors.Add(new BslValidationError(error.Key, errorText));
      }

      var exception = BslUserExceptionFactory<BslUserException>.CreateException(
        this.GetType(),
        typeof(BslServiceBaseStrings),
        (int)BslServiceBaseStrings.EntityValidationError,
        sb.ToString(),
        innerException);

      exception.Errors = validationErrors;
      exception.EntityObject = entity;

      throw exception;
    }

    #endregion Methods
  }
}