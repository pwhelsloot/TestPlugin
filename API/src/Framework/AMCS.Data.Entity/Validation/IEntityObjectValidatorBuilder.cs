using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  public interface IEntityObjectValidatorBuilder
  {
    /// <summary>
    /// Adds the property fail validation test.
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="validatedProperty">The validated property.</param>
    /// <param name="failValidationTest">The fail validation test.</param>
    /// <param name="overwrite">Overwrite an existing validation test if it already exists?</param>
    void AddPropertyFailValidationTest(Type entityType, string validatedProperty, string failValidationTest, string errorMessage, bool overwrite = false);

    /// <summary>
    /// Adds the property fail validation test.
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="propertyValidation">The property validation to be added.</param>
    /// <param name="overwrite">Overwrite an existing validation test if it already exists?</param>
    void AddPropertyFailValidationTest(Type entityType, EntityPropertyValidationEntity propertyValidation, bool overwrite = false);

    void Build();
  }
}