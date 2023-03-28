using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  internal class PropertyValidation
  {
    private readonly AMCS.Data.EntityValidation.Rules.Validation _validation;
    private readonly string _error;

    public PropertyValidation(AMCS.Data.EntityValidation.Rules.Validation validation, string error)
    {
      _validation = validation;
      _error = error;
    }

    public string Validate(object entity)
    {
      if (!_validation.IsValid(entity))
        return _error;
      return null;
    }
  }
}
