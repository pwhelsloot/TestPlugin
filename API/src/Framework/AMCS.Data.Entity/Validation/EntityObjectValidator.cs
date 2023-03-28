//-----------------------------------------------------------------------------
// <copyright file="EntityObjectValidator.cs" company="AMCS Group">
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

namespace AMCS.Data.Entity.Validation
{
  public class EntityObjectValidator : IEntityObjectValidator
  {
    private readonly Dictionary<Type, Dictionary<string, ValidationTest>> propertyTests = new Dictionary<Type, Dictionary<string, ValidationTest>>();
    private readonly Dictionary<(Type Type, int ContextId), Dictionary<string, ValidationTest>> contextPropertyTests = new Dictionary<(Type Type, int ContextId), Dictionary<string, ValidationTest>>();
    private readonly Dictionary<Type, EntityValidation> validations = new Dictionary<Type, EntityValidation>();
    private readonly Dictionary<(Type Type, int ContextId), EntityValidation> contextValidations = new Dictionary<(Type Type, int ContextId), EntityValidation>();

    /// <summary>
    /// Returns true if the type has validation rules defined in its mapping file.
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <returns>
    /// 	<c>true</c> if [is type validated] [the specified entity type]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsTypeValidated(Type entityType)
    {
      return propertyTests.ContainsKey(entityType);
    }

    /// <summary>
    /// Returns true if the type has validation rules defined either globally or in the given context.
    /// </summary>
    public bool IsTypeValidated(Type entityType, int validationContextId)
    {
      return
        propertyTests.ContainsKey(entityType) ||
        contextPropertyTests.ContainsKey((entityType, validationContextId));
    }

    /// <summary>
    /// Determines whether [is type property validated] [the specified entity type].
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    /// 	<c>true</c> if [is type property validated] [the specified entity type]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsTypePropertyValidated(Type entityType, string propertyName)
    {
      return
        propertyTests.TryGetValue(entityType, out var tests) &&
        tests.ContainsKey(propertyName);
    }

    /// <summary>
    /// Determines whether the type property is validated - either globally or in the given context.
    /// </summary>
    public bool IsTypePropertyValidated(Type entityType, int validationContextId, string propertyName)
    {
      if (
        propertyTests.TryGetValue(entityType, out var tests) &&
        tests.ContainsKey(propertyName))
      {
        return true;
      }

      // for contextual rules, only accept those where the fail test is non-null
      return
        contextPropertyTests.TryGetValue((entityType, validationContextId), out var contextTests) &&
        contextTests.TryGetValue(propertyName, out var entityRule) &&
        !string.IsNullOrWhiteSpace(entityRule.Test);
    }

    /// <summary>
    /// Gets the validated properties.
    /// </summary>
    /// <returns></returns>
    public string[] GetValidatedProperties(Type entityType)
    {
      if (propertyTests.TryGetValue(entityType, out var propertyFailTest))
        return propertyFailTest.Keys.ToArray();

      return null;
    }

    /// <summary>
    /// Gets the validated properties.
    /// </summary>
    /// <returns></returns>
    public string[] GetValidatedProperties(Type entityType, int validationContextId)
    {
      var validatedProperties = new List<string>();

      if (propertyTests.TryGetValue(entityType, out var propertyRules))
        validatedProperties.AddRange(propertyRules.Keys);

      if (contextPropertyTests.TryGetValue((entityType, validationContextId), out var entityRules))
        validatedProperties.AddRange(entityRules.Keys);

      if (validatedProperties.Count > 0)
        return validatedProperties.ToArray();

      return null;
    }

    /// <summary>
    /// Gets the property validation error.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public string GetPropertyValidationError(EntityObject entity, string propertyName)
    {
      if (validations.TryGetValue(entity.GetType(), out var valMethod))
        return valMethod.Validate(entity, propertyName);

      return null;
    }

    /// <summary>
    /// Gets the property validation error.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public string GetPropertyValidationError(EntityObject entity, int validationContextId, string propertyName)
    {
      if (contextValidations.TryGetValue((entity.GetType(), validationContextId), out var validation))
        return validation.Validate(entity, propertyName);

      return null;
    }

    public IEntityObjectValidatorBuilder CreateBuilder()
    {
      return new Builder(this);
    }

    private class Builder : IEntityObjectValidatorBuilder
    {
      private readonly EntityObjectValidator validator;

      public Builder(EntityObjectValidator validator)
      {
        this.validator = validator;
      }

      /// <summary>
      /// Adds the property fail validation test.
      /// </summary>
      /// <param name="entityType">Type of the entity.</param>
      /// <param name="validatedProperty">The validated property.</param>
      /// <param name="failValidationTest">The fail validation test.</param>
      /// <param name="overwrite">Overwrite an existing validation test if it already exists?</param>
      public void AddPropertyFailValidationTest(Type entityType, string validatedProperty, string failValidationTest, string errorMessage, bool overwrite = false)
      {
        if (failValidationTest == null)
          return;

        if (!validator.propertyTests.TryGetValue(entityType, out var tests))
        {
          tests = new Dictionary<string, ValidationTest>();
          validator.propertyTests.Add(entityType, tests);
        }

        if (tests.ContainsKey(validatedProperty) && !overwrite)
          return;

        tests[validatedProperty] = new ValidationTest(failValidationTest, errorMessage);
      }

      /// <summary>
      /// Adds the property fail validation test.
      /// </summary>
      /// <param name="entityType">Type of the entity.</param>
      /// <param name="propertyValidation">The property validation to be added.</param>
      /// <param name="overwrite">Overwrite an existing validation test if it already exists?</param>
      public void AddPropertyFailValidationTest(Type entityType, EntityPropertyValidationEntity propertyValidation, bool overwrite = false)
      {
        if (propertyValidation.FailValidationTest == null)
          return;

        var key = (entityType, propertyValidation.ValidationContextId.Value);
        if (!validator.contextPropertyTests.TryGetValue(key, out var tests))
        {
          tests = new Dictionary<string, ValidationTest>();
          validator.contextPropertyTests.Add(key, tests);
        }

        if (tests.ContainsKey(propertyValidation.PropertyName) && !overwrite)
          return;

        tests[propertyValidation.PropertyName] = new ValidationTest(propertyValidation.FailValidationTest, propertyValidation.ErrorText);
      }

      public void Build()
      {
        var entityTypes = new HashSet<Type>(
          validator.propertyTests.Keys.Concat(validator.contextPropertyTests.Keys.Select(p => p.Type))
        );
        var contextIds = new HashSet<int>(
          validator.contextPropertyTests.Keys.Select(p => p.ContextId)
        );

        foreach (var entityType in entityTypes)
        {
          Dictionary<string, PropertyValidation> validations = null;
          if (validator.propertyTests.TryGetValue(entityType, out var tests))
          {
            validations = BuildPropertyValidations(entityType, tests);
            validator.validations.Add(entityType, new EntityValidation(validations, null));
          }

          foreach (int contextId in contextIds)
          {
            Dictionary<string, PropertyValidation> contextValidations = null;
            if (validator.contextPropertyTests.TryGetValue((entityType, contextId), out var contextTests))
              contextValidations = BuildPropertyValidations(entityType, contextTests);

            if (validations != null || contextValidations != null)
              validator.contextValidations.Add((entityType, contextId), new EntityValidation(validations, contextValidations));
          }
        }
      }

      private Dictionary<string, PropertyValidation> BuildPropertyValidations(Type entityType, Dictionary<string, ValidationTest> propertyRules)
      {
        if (propertyRules == null)
          return null;

        var propertyValidations = new Dictionary<string, PropertyValidation>();

        foreach (var propertyRule in propertyRules)
        {
          if (!string.IsNullOrEmpty(propertyRule.Value.Test))
          {
            // Currently there is no need to actually return a meaningful error string because the messages from mappings files are used.

            propertyValidations.Add(
              propertyRule.Key,
              new PropertyValidation(
                AMCS.Data.EntityValidation.Rules.Validation.Parse(entityType, propertyRule.Value.Test),
                propertyRule.Value.Error));
          }
        }

        return propertyValidations;
      }
    }
  }
}
