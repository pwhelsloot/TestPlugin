using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  internal class EntityValidation
  {
    private readonly Dictionary<string, PropertyValidation> properties;
    private readonly Dictionary<string, PropertyValidation> propertiesInContext;

    public EntityValidation(Dictionary<string, PropertyValidation> properties, Dictionary<string, PropertyValidation> propertiesInContext)
    {
      this.properties = properties;
      this.propertiesInContext = propertiesInContext;
    }

    public string Validate(object entity, string propertyName)
    {
      string error = null;

      if (properties != null && properties.TryGetValue(propertyName, out var validation))
        error = validation.Validate(entity);

      if (error == null && propertiesInContext != null && propertiesInContext.TryGetValue(propertyName, out validation))
        error = validation.Validate(entity);

      return error;
    }
  }
}