using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  internal class EntityParameterInfo : ParameterInfo
  {
    public override Type ParameterType { get; }

    public override object DefaultValue
    {
      get
      {
        if (ParameterType.IsValueType)
          return Activator.CreateInstance(ParameterType);

        return null;
      }
    }

    public EntityParameterInfo(Type entityType)
    {
      ParameterType = entityType;
    }
  }
}
