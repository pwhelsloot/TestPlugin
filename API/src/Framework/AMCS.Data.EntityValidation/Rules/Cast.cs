using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal abstract partial class Cast
  {
    private static readonly Dictionary<Type, Cast> _casts = BuildCasts();

    public static Cast GetCast(Type type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof(type));

      type = Nullable.GetUnderlyingType(type) ?? type;

      _casts.TryGetValue(type, out var cast);

      return cast;
    }

    public abstract Type ReturnType { get; }

    public abstract object GetValue(object value);
  }
}
