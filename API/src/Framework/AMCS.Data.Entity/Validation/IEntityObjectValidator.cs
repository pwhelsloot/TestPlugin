using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  public interface IEntityObjectValidator
  {
    /// <summary>
    /// Returns true if the type has validation rules defined in its mapping file.
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <returns>
    /// 	<c>true</c> if [is type validated] [the specified entity type]; otherwise, <c>false</c>.
    /// </returns>
    bool IsTypeValidated(Type entityType);

    /// <summary>
    /// Returns true if the type has validation rules defined either globally or in the given context.
    /// </summary>
    bool IsTypeValidated(Type entityType, int validationContextId);

    /// <summary>
    /// Determines whether [is type property validated] [the specified entity type].
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    /// 	<c>true</c> if [is type property validated] [the specified entity type]; otherwise, <c>false</c>.
    /// </returns>
    bool IsTypePropertyValidated(Type entityType, string propertyName);

    /// <summary>
    /// Determines whether the type property is validated - either globally or in the given context.
    /// </summary>
    bool IsTypePropertyValidated(Type entityType, int validationContextId, string propertyName);

    /// <summary>
    /// Gets the validated properties.
    /// </summary>
    /// <returns></returns>
    string[] GetValidatedProperties(Type entityType);

    /// <summary>
    /// Gets the validated properties.
    /// </summary>
    /// <returns></returns>
    string[] GetValidatedProperties(Type entityType, int validationContextId);

    /// <summary>
    /// Gets the property validation error.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    string GetPropertyValidationError(EntityObject entity, string propertyName);

    /// <summary>
    /// Gets the property validation error.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    string GetPropertyValidationError(EntityObject entity, int validationContextId, string propertyName);
  }
}