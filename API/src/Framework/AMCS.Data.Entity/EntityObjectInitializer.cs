using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AMCS.Data.Entity.Interfaces;
using AMCS.Data.Support;

namespace AMCS.Data.Entity
{
  internal class EntityObjectInitializer
  {
    private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType), EntityObjectInitializer> Initializers = new ConcurrentDictionary<(Type SourceType, Type TargetType), EntityObjectInitializer>();
    // Synchronized dictionary to ensure no duplicates are built.
    private static readonly Dictionary<(Type SourceType, Type TargetType), EntityObjectInitializer> BuiltInitializers = new Dictionary<(Type SourceType, Type TargetType), EntityObjectInitializer>();
    private static readonly object SyncRoot = new object();

    private static readonly Func<(Type SourceType, Type TargetType), EntityObjectInitializer> BuildEntityObjectInitializerDelegate = BuildEntityObjectInitializer;

    public static EntityObjectInitializer ForTypes(Type sourceType, Type targetType)
    {
      return Initializers.GetOrAdd((sourceType, targetType), BuildEntityObjectInitializerDelegate);
    }

    private static EntityObjectInitializer BuildEntityObjectInitializer((Type SourceType, Type TargetType) pair)
    {
      // ConcurrentDictionary`2.GetOrAdd does not synchronize the callbacks. However, we can only
      // ever instantiate an EntityObjectAccessor once because of the proxy types it create.
      // So, we synchronize the builders here.

      lock (SyncRoot)
      {
        if (!BuiltInitializers.TryGetValue(pair, out var initializer))
        {
          initializer = new EntityObjectInitializer(pair);
          BuiltInitializers.Add(pair, initializer);
        }

        return initializer;
      }
    }

    private readonly Action<object, object>[] initializers;

    private EntityObjectInitializer((Type SourceType, Type TargetType) pair)
    {
      // For normal entities, we require the entities to inherit, either source from
      // target or target from source.

      bool canInitialize = pair.SourceType.IsAssignableFrom(pair.TargetType) || pair.TargetType.IsAssignableFrom(pair.SourceType);

      // For partial entities, we require the source type to be the exact complete
      // entity type.

      bool isPartialEntity = typeof(IPartialEntity).IsAssignableFrom(pair.TargetType);
      if (isPartialEntity && !canInitialize)
      {
        var completeEntityType = ((IPartialEntity)Activator.CreateInstance(pair.TargetType)).GetCompleteEntityType();
        canInitialize = completeEntityType == pair.SourceType;
      }

      if (!canInitialize)
        throw new Exception($"System Error: Cannot construct {pair.TargetType} from {pair.SourceType}");

      var initializers = new List<Action<object, object>>();

      foreach (var targetProperty in pair.TargetType.GetProperties())
      {
        var sourceProperty = pair.SourceType.GetProperty(targetProperty.Name);
        if (sourceProperty == null)
          continue;

        if (targetProperty.PropertyType == typeof(EntityBlob))
        {
          var sourceGetter = ReflectionHelper.GetPropertyGetter(sourceProperty);
          var targetGetter = ReflectionHelper.GetPropertyGetter(targetProperty);

          initializers.Add(CreateEntityBlobInitializer(sourceGetter, targetGetter));
        }
        else if (targetProperty.GetSetMethod() != null)
        {
          if (isPartialEntity && sourceProperty.PropertyType.FullName.Contains("ObservableCollection"))
            continue;

          var sourceGetter = ReflectionHelper.GetPropertyGetter(sourceProperty);
          var targetSetter = ReflectionHelper.GetPropertySetter(targetProperty);

          // If the source and target properties are value types, and the source is a nullable
          // value type and the target is not, we need to provide a default value if the
          // source property is null.

          Func<object> defaultValue = null;

          if (
            sourceProperty.PropertyType.IsValueType &&
            targetProperty.PropertyType.IsValueType &&
            Nullable.GetUnderlyingType(sourceProperty.PropertyType) != null &&
            Nullable.GetUnderlyingType(targetProperty.PropertyType) == null)
          {
            defaultValue = ReflectionHelper.GetDefaultValueFactory(targetProperty.PropertyType);
          }

          initializers.Add(CreateCopyInitializer(sourceGetter, targetSetter, defaultValue));
        }
      }

      this.initializers = initializers.ToArray();
    }

    private Action<object, object> CreateEntityBlobInitializer(Func<object, object> sourceGetter, Func<object, object> targetGetter)
    {
      return (source, target) =>
      {
        var sourceBlob = (EntityBlob)sourceGetter(source);
        if (sourceBlob.TryGetPendingBlob(out var data))
        {
          var targetBlob = (EntityBlob)targetGetter(target);
          targetBlob.SetPendingBlob(data);
        }
      };
    }

    private Action<object, object> CreateCopyInitializer(Func<object, object> sourceGetter, Action<object, object> targetSetter, Func<object> defaultValue)
    {
      return (source, target) =>
      {
        var value = sourceGetter(source);
        if (value == null)
          value = defaultValue?.Invoke();

        targetSetter(target, value);
      };
    }

    public void Initialize(object source, object target)
    {
      foreach (var initializer in initializers)
      {
        initializer(source, target);
      }
    }
  }
}
