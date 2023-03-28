namespace AMCS.Data.Support
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;
  using System.Reflection;

  [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Improve readibility of heavily nested expression builders")]
  [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Improve readibility of heavily nested expression builders")]
  public static class ReflectionHelper
  {
    private static readonly Dictionary<Type, Func<object>> Factories = new Dictionary<Type, Func<object>>();
    private static readonly Dictionary<PropertyInfo, Func<object, object>> PropertyGetters = new Dictionary<PropertyInfo, Func<object, object>>();
    private static readonly Dictionary<PropertyInfo, Action<object, object>> PropertySetters = new Dictionary<PropertyInfo, Action<object, object>>();
    private static readonly Dictionary<PropertyInfo, Action<object, object>> EntityPropertySetters = new Dictionary<PropertyInfo, Action<object, object>>();
    private static readonly object SyncRoot = new object();
    private static bool disableILGeneration;

    internal static void DisableILGeneration(bool disabled)
    {
      if (disableILGeneration != disabled)
      {
        disableILGeneration = disabled;
        Factories.Clear();
        PropertyGetters.Clear();
        PropertySetters.Clear();
        EntityPropertySetters.Clear();
      }
    }

    /// <summary>
    /// Build a factory to create default value instances of the provided value type.
    /// </summary>
    /// <param name="type">The type to create the factory for.</param>
    /// <returns>The factory to create instances of the provided type.</returns>
    /// <remarks>
    /// This method returns a runtime compiled expression to construct a default value
    /// for a value type. The reason this is done like this instead of through
    /// <see cref="Activator.CreateInstance(Type)"/> is that method is roughly 14x as fast
    /// as the <see cref="Activator.CreateInstance(Type)"/> method. Since we're using
    /// this in hot paths in the SQL helper methods, it's important this is fast.
    /// 
    /// Below are the results of the benchmarks. The ActivatorCreateInstanceBenchmark's are the
    /// ones that use <see cref="Activator.CreateInstance(Type)"/>, and the
    /// ExpressionBenchmark's are the ones that use the below method. The Int64 ones create
    /// a <see cref="Int64"/> instance and the MyLargeStruct ones create a struct of 24 bytes.
    /// 
    ///                                            Type |         Method |      Mean |     Error |    StdDev |
    /// ----------------------------------------------- |--------------- |----------:|----------:|----------:|
    ///          ActivatorCreateInstanceBenchmark_Int64 | CreateInstance | 58.195 ns | 1.2360 ns | 1.1562 ns |
    ///  ActivatorCreateInstanceBenchmark_MyLargeStruct | CreateInstance | 59.670 ns | 1.2297 ns | 1.6833 ns |
    ///                       ExpressionBenchmark_Int64 | CreateInstance |  4.062 ns | 0.1246 ns | 0.1105 ns |
    ///               ExpressionBenchmark_MyLargeStruct | CreateInstance |  6.908 ns | 0.2018 ns | 0.2829 ns |
    /// 
    /// 
    /// </remarks>
    public static Func<object> GetDefaultValueFactory(Type type)
    {
      lock (SyncRoot)
      {
        if (!Factories.TryGetValue(type, out var factory))
        {
          if (disableILGeneration)
            factory = DirectDefaultValue(type);
          else
            factory = BuildDefaultValueFactory(type).Compile();
          Factories.Add(type, factory);
        }

        return factory;
      }
    }

    private static Func<object> DirectDefaultValue(Type type)
    {
      return new Func<object>(() => type.IsValueType ? Activator.CreateInstance(type) : null);
    }

    private static Expression<Func<object>> BuildDefaultValueFactory(Type type)
    {
      // Taken from https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype/12733445#12733445.

      var expression = Expression.Lambda<Func<object>>(
        Expression.Convert(
          Expression.Default(type), typeof(object)
        )
      );

      return expression;
    }

    public static Func<object, object> GetPropertyGetter(Type type, string name)
    {
      var property = type.GetProperty(name);

      return GetPropertyGetter(property);
    }

    public static Func<object, object> GetPropertyGetter(PropertyInfo property)
    {
      lock (SyncRoot)
      {
        if (!PropertyGetters.TryGetValue(property, out var getter))
        {
          if (disableILGeneration)
            getter = DirectPropertyGetter(property);
          else
            getter = BuildPropertyGetter(property).Compile();
          PropertyGetters.Add(property, getter);
        }
        return getter;
      }
    }

    private static Func<object, object> DirectPropertyGetter(PropertyInfo propertyInfo)
    {
      if (propertyInfo.GetMethod == null)
        return new Func<object, object>((instance) => throw new NotSupportedException());

      return new Func<object, object>((instance) => propertyInfo.GetMethod.Invoke(instance, null));
    }

    private static Expression<Func<object, object>> BuildPropertyGetter(PropertyInfo property)
    {
      var target = Expression.Parameter(typeof(object), "target");

      if (property.GetMethod == null)
      {
        return Expression.Lambda<Func<object, object>>(
          Expression.Throw(
            Expression.New(typeof(NotSupportedException))
          ),
          target
        );
      }

      return Expression.Lambda<Func<object, object>>(
        Expression.Convert(
          Expression.Call(
            Expression.Convert(target, property.DeclaringType),
            property.GetMethod
          ),
          typeof(object)
        ),
        target
      );
    }

    public static Action<object, object> GetPropertySetter(Type type, string name)
    {
      var property = type.GetProperty(name);

      return GetPropertySetter(property);
    }

    public static Action<object, object> GetPropertySetter(PropertyInfo property)
    {
      lock (SyncRoot)
      {
        if (!PropertySetters.TryGetValue(property, out var setter))
        {
          if (disableILGeneration)
            setter = DirectPropertySetter(property);
          else
            setter = BuildPropertySetter(property).Compile();
          PropertySetters.Add(property, setter);
        }
        return setter;
      }
    }

    private static Action<object, object> DirectPropertySetter(PropertyInfo propertyInfo)
    {
      if (propertyInfo.SetMethod == null)
        return new Action<object, object>((instance, value) => throw new NotSupportedException());

      return new Action<object, object>((instance, value) => propertyInfo.SetMethod.Invoke(instance, new[] { value }));
    }

    private static Expression<Action<object, object>> BuildPropertySetter(PropertyInfo property)
    {
      var target = Expression.Parameter(typeof(object), "target");
      var value = Expression.Parameter(typeof(object), "value");

      if (property.SetMethod == null)
      {
        return Expression.Lambda<Action<object, object>>(
          Expression.Throw(
            Expression.New(typeof(NotSupportedException))
          ),
          target,
          value
        );
      }

      return Expression.Lambda<Action<object, object>>(
        Expression.Call(
          Expression.Convert(target, property.DeclaringType),
          property.SetMethod,
          Expression.Convert(value, property.PropertyType)
        ),
        target,
        value
      );
    }

    public static Action<object, object> GetEntityPropertySetter(Type type, string name)
    {
      var property = type.GetProperty(name);

      return GetEntityPropertySetter(property);
    }

    public static Action<object, object> GetEntityPropertySetter(PropertyInfo property)
    {
      lock (SyncRoot)
      {
        if (!EntityPropertySetters.TryGetValue(property, out var setter))
        {
          if (disableILGeneration)
            setter = DirectEntityPropertySetter(property);
          else
            setter = BuildEntityPropertySetter(property).Compile();
          EntityPropertySetters.Add(property, setter);
        }
        return setter;
      }
    }

    private static Action<object, object> DirectEntityPropertySetter(PropertyInfo propertyInfo)
    {
      if (propertyInfo.SetMethod == null)
        return new Action<object, object>((instance, value) => throw new NotSupportedException());

      bool isNullable =
        !propertyInfo.PropertyType.IsValueType ||
        Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;

      bool coerceStringToChar = propertyInfo.PropertyType == typeof(char) || propertyInfo.PropertyType == typeof(char?);

      var propertyType = propertyInfo.PropertyType;
      var unwrappedType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

      return new Action<object, object>((instance, value) =>
      {
        object correctedValue = value;
        if (!isNullable && value == null)
        {
          correctedValue = GetDefaultValueFactory(propertyInfo.PropertyType).Invoke();
        }

        if (coerceStringToChar && correctedValue is string)
        {
          correctedValue = ((string)correctedValue)[0];
        }

        if (!unwrappedType.IsEnum)
        {
          if (value != null && value.GetType() != unwrappedType)
            correctedValue = ValueCoercion.Coerce(correctedValue, unwrappedType);
        }

        propertyInfo.SetMethod.Invoke(instance, new[] { correctedValue });
      });
    }

    private static Expression<Action<object, object>> BuildEntityPropertySetter(PropertyInfo property)
    {
      var target = Expression.Parameter(typeof(object), "target");
      var value = Expression.Parameter(typeof(object), "value");

      if (property.SetMethod == null)
      {
        return Expression.Lambda<Action<object, object>>(
          Expression.Throw(
            Expression.New(typeof(NotSupportedException))
          ),
          target,
          value
        );
      }

      var expressions = new List<Expression>();

      bool isNullable =
        !property.PropertyType.IsValueType ||
        Nullable.GetUnderlyingType(property.PropertyType) != null;

      if (!isNullable)
      {
        // if (value == null)
        //   value = (object)default(TProperty)

        expressions.Add(
          Expression.IfThen(
            Expression.Equal(
              value,
              Expression.Constant(null)
            ),
            Expression.Assign(
              value,
              Expression.Convert(Expression.Default(property.PropertyType), typeof(object))
            )
          )
        );
      }

      bool coerceStringToChar = property.PropertyType == typeof(char) || property.PropertyType == typeof(char?);

      if (coerceStringToChar)
      {
        // if (value is string)
        //   value = (object)((string)value)[0];

        var indexMethod = typeof(string).GetProperty("Chars");

        expressions.Add(
          Expression.IfThen(
            Expression.TypeIs(value, typeof(string)),
            Expression.Assign(
              value,
              Expression.Convert(
                Expression.Call(
                  Expression.Convert(value, typeof(string)),
                  indexMethod.GetMethod,
                  Expression.Constant(0)
                ),
                typeof(object)
              )
            )
          )
        );
      }

      // if (value != null && value.GetType() != unwrappedType /* lifted: && !unwrappedType.IsEnum */)
      //   value = ValueCoercion.Coerce(value, unwrappedType)

      var unwrappedType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

      if (!unwrappedType.IsEnum)
      {
        var getType = typeof(object).GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance);
        var coerce = typeof(ValueCoercion).GetMethod("Coerce", BindingFlags.Public | BindingFlags.Static);

        expressions.Add(
          Expression.IfThen(
            Expression.AndAlso(
              Expression.NotEqual(
                value,
                Expression.Constant(null)
              ),
              Expression.NotEqual(
                Expression.Call(value, getType),
                Expression.Constant(unwrappedType, typeof(Type))
              )
            ),
            Expression.Assign(
              value,
              Expression.Call(
                coerce,
                value,
                Expression.Constant(unwrappedType, typeof(Type))
              )
            )
          )
        );
      }

      // target.Property = value;

      expressions.Add(
        Expression.Call(
          Expression.Convert(target, property.DeclaringType),
          property.SetMethod,
          Expression.Convert(value, property.PropertyType)
        )
      );

      return Expression.Lambda<Action<object, object>>(
        Expression.Block(expressions),
        target,
        value
      );
    }
  }
}
