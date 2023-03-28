using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public class EntityObjectMapperBuilder : IEntityObjectMapperBuilder, IDelayedStartup
  {
    private readonly List<Action<EntityObjectMapperBuilder>> actions = new List<Action<EntityObjectMapperBuilder>>();
    private readonly Dictionary<(Type From, Type To), IMapperBuilder> builders = new Dictionary<(Type From, Type To), IMapperBuilder>();
    private readonly object syncRoot = new object();

    public void AddAction(Action<EntityObjectMapperBuilder> action)
    {
      lock (syncRoot)
      {
        this.actions.Add(action);
      }
    }

    public void Start()
    {
      List<Action<EntityObjectMapperBuilder>> actions;
      lock (syncRoot)
      {
        actions = this.actions.ToList();
      }
      foreach (var action in actions)
      {
        action?.Invoke(this);
      }
    }

    public EntityObjectMapperBuilder CreateMap<TFrom, TTo>(Func<IEntityObjectEntityMapperBuilder<TFrom, TTo>, IEntityObjectEntityMapperBuilder<TFrom, TTo>> configure = null)
    {
      var builder = new MapperBuilder<TFrom, TTo>();

      if (configure != null)
        builder = (MapperBuilder<TFrom, TTo>)configure.Invoke(builder);

      var key = (typeof(TFrom), typeof(TTo));
      if (builders.ContainsKey(key))
        throw new ArgumentException($"Duplicate mapping of type '{typeof(TFrom)}' to '{typeof(TTo)}'");

      builders.Add(key, builder);

      return this;
    }
    
    public IEntityObjectMapper Build()
    {
      return new EntityObjectMapper(builders.Values.Select(p => p.GetMapper(this)), this);
    }

    private interface IMapperBuilder
    {
      Mapper GetMapper(EntityObjectMapperBuilder builder);
    }

    private class MapperBuilder<TFrom, TTo> : IEntityObjectEntityMapperBuilder<TFrom, TTo>, IMapperBuilder
    {
      private readonly EntityObjectAccessor fromAccessor = EntityObjectAccessor.ForType(typeof(TFrom));
      private readonly EntityObjectAccessor toAccessor = EntityObjectAccessor.ForType(typeof(TTo));
      private readonly Dictionary<EntityObjectProperty, MappingPropertyBuilder<TFrom, TTo>> propertyMappings = new Dictionary<EntityObjectProperty, MappingPropertyBuilder<TFrom, TTo>>();
      private Action<object, object> beforeMap;
      private Action<object, object> afterMap;

      public IEntityObjectEntityMapperBuilder<TFrom, TTo> Map(string property, Action<IEntityObjectPropertyMapperBuilder<TFrom, TTo>> configure)
      {
        var toProperty = toAccessor.GetProperty(property);
        if (toProperty == null)
          throw new ArgumentException($"Unknown property '{property}' on '{typeof(TTo)}'");

        Map(toProperty, configure);

        return this;
      }

      public IEntityObjectEntityMapperBuilder<TFrom, TTo> MapColumn(string column, Action<IEntityObjectPropertyMapperBuilder<TFrom, TTo>> configure)
      {
        var toProperty = toAccessor.GetPropertyByColumnName(column);
        if (toProperty == null)
          throw new ArgumentException($"Unknown column '{column}' on '{typeof(TTo)}'");

        Map(toProperty, configure);

        return this;
      }

      private void Map(EntityObjectProperty property, Action<IEntityObjectPropertyMapperBuilder<TFrom, TTo>> configure)
      {
        if (propertyMappings.ContainsKey(property))
          throw new ArgumentException($"Property '{property.Name}' is already mapped");

        var propertyBuilder = new MappingPropertyBuilder<TFrom, TTo>(fromAccessor, property);

        propertyMappings.Add(property, propertyBuilder);

        configure(propertyBuilder);
      }

      public IEntityObjectEntityMapperBuilder<TFrom, TTo> BeforeMap(Action<TFrom, TTo> action)
      {
        if (beforeMap != null)
          throw new ArgumentException("BeforeMap already assigned");

        beforeMap = (from, to) => action((TFrom)from, (TTo)to);

        return this;
      }

      public IEntityObjectEntityMapperBuilder<TFrom, TTo> AfterMap(Action<TFrom, TTo> action)
      {
        if (afterMap != null)
          throw new ArgumentException("AfterMap already assigned");

        afterMap = (from, to) => action((TFrom)from, (TTo)to);

        return this;
      }

      public Mapper GetMapper(EntityObjectMapperBuilder builder)
      {
        // Find all target properties we don't have a mapping for and map them default.

        foreach (var property in toAccessor.Properties)
        {
          if (!property.CanWrite)
            continue;

          if (propertyMappings.ContainsKey(property))
            continue;

          var fromProperty = fromAccessor.GetProperty(property.Name);
          if (fromProperty == null || !fromProperty.CanRead)
            continue;

          // Perform an implicit mapping.

          var propertyBuilder = new MappingPropertyBuilder<TFrom, TTo>(fromAccessor, property);

          propertyMappings.Add(property, propertyBuilder);

          propertyBuilder.MapImplicit(fromProperty, builder);
        }

        return new Mapper(
          typeof(TFrom),
          typeof(TTo),
          beforeMap,
          afterMap,
          propertyMappings
            .Where(p => !p.Value.Ignored)
            .Select(p => p.Value.GetMapper()));
      }
    }

    private class MappingPropertyBuilder<TFrom, TTo> : IEntityObjectPropertyMapperBuilder<TFrom, TTo>
    {
      private readonly EntityObjectAccessor fromAccessor;
      private EntityObjectProperty fromProperty;
      private readonly EntityObjectProperty toProperty;
      private Func<IEntityObjectMapper, object, object, object> valueSource;

      public bool Ignored { get; private set; }

      public MappingPropertyBuilder(EntityObjectAccessor fromAccessor, EntityObjectProperty property)
      {
        this.fromAccessor = fromAccessor;
        toProperty = property;
      }

      public void MapFrom(string property)
      {
        var fromProperty = fromAccessor.GetProperty(property);
        if (fromProperty == null)
          throw new ArgumentException($"Unknown property '{property}' on '{typeof(TFrom)}'");

        MapFrom(fromProperty);
      }

      public void MapFromColumn(string column)
      {
        var fromProperty = fromAccessor.GetPropertyByColumnName(column);
        if (fromProperty == null)
          throw new ArgumentException($"Unknown column '{column}' on '{typeof(TFrom)}'");

        MapFrom(fromProperty);
      }

      private void MapFrom(EntityObjectProperty property)
      {
        this.fromProperty = property;

        valueSource = (mapper, from, to) => property.GetValue(from);
      }

      public void MapFrom(Func<TFrom, object> func)
      {
        valueSource = (mapper, from, to) => func((TFrom)from);
      }

      public void MapFrom(Func<TFrom, TTo, object> func)
      {
        valueSource = (mapper, from, to) => func((TFrom)from, (TTo)to);
      }

      public void Ignore()
      {
        Ignored = true;
      }

      public void MapImplicit(EntityObjectProperty fromProperty, EntityObjectMapperBuilder builder)
      {
        // Are source and target a list type?

        this.fromProperty = fromProperty;

        var fromElementType = GetElementType(fromProperty.Type);
        var toElementType = GetElementType(toProperty.Type);

        if (fromElementType != null && toElementType != null)
        {
          if (builder.IsMapped(fromElementType, toElementType))
          {
            valueSource = fromProperty.Type.IsArray
              ? BuildArrayMapper(fromProperty, toElementType)
              : BuildListMapper(fromProperty, toElementType);
          }
        }
        else
        {
          if (builder.IsMapped(fromProperty.Type, toProperty.Type))
            valueSource = BuildMapper(fromProperty, toProperty.Type);
        }

        if (valueSource == null)
          valueSource = BuildValueMapper(fromProperty);
      }

      private static Func<IEntityObjectMapper, object, object, object> BuildListMapper(EntityObjectProperty fromProperty, Type toElementType)
      {
        var toListType = typeof(List<>).MakeGenericType(toElementType);

        return (mapper, from, to) =>
        {
          var fromList = (IEnumerable)fromProperty.GetValue(from);
          if (fromList == null)
            return null;

          var toList = (IList)Activator.CreateInstance(toListType);

          mapper.MapList(fromList, toElementType, toList);

          return toList;
        };
      }

      private static Func<IEntityObjectMapper, object, object, object> BuildArrayMapper(EntityObjectProperty fromProperty, Type toElementType)
      {
        var toArrayType = toElementType;

        return (mapper, from, to) =>
        {
          var fromArray = (Array)fromProperty.GetValue(from);
          if (fromArray == null)
            return null;

          var toArray = Array.CreateInstance(toArrayType, fromArray.Length);

          mapper.MapArray(fromArray, toElementType, toArray);

          return toArray;
        };
      }

      private static Func<IEntityObjectMapper, object, object, object> BuildMapper(EntityObjectProperty fromProperty, Type toType)
      {
        return (mapper, from, to) =>
        {
          var fromValue = fromProperty.GetValue(from);
          if (fromValue == null)
            return null;

          return mapper.Map(fromValue, toType);
        };
      }

      private static Func<IEntityObjectMapper, object, object, object> BuildValueMapper(EntityObjectProperty fromProperty)
      {
        return (mapper, from, to) => fromProperty.GetValue(from);
      }

      private Type GetElementType(Type type)
      {
        if (type != typeof(string) && type != typeof(byte[]))
        {
          if (typeof(IEnumerable<>).IsAssignableFromGeneric(type))
          {
            return type.GetGenericTypeArguments(typeof(IEnumerable<>))[0];
          }
        }

        return null;
      }

      public MappingProperty GetMapper()
      {
        return new MappingProperty(fromProperty, toProperty, valueSource);
      }
    }

    private bool IsMapped(Type from, Type to)
    {
      for (;  from != null; from = from.BaseType)
      {
        if (builders.ContainsKey((from, to)))
          return true;
      }

      return false;
    }

    private class Mapper
    {
      private readonly Action<object, object> beforeMap;
      private readonly Action<object, object> afterMap;
      private readonly MappingProperty[] propertyMappings;
      private readonly Dictionary<EntityObjectProperty, MappingProperty> propertyMappingsByFromProperty;

      public Type FromType { get; }
      public Type ToType { get; }

      public Mapper(Type fromType, Type toType, Action<object, object> beforeMap, Action<object, object> afterMap, IEnumerable<MappingProperty> propertyMappings)
      {
        this.beforeMap = beforeMap;
        this.afterMap = afterMap;
        this.propertyMappings = propertyMappings.ToArray();

        propertyMappingsByFromProperty = this.propertyMappings
          .Where(p => p.FromProperty != null)
          .ToDictionary(p => p.FromProperty, p => p);

        FromType = fromType;
        ToType = toType;
      }

      public void Map(object value, object result, IEntityObjectMapper mapper)
      {
        beforeMap?.Invoke(value, result);

        foreach (var mapping in propertyMappings)
        { 
          mapping.ToProperty.SetValue(result, mapping.ValueSource(mapper, value, result));
        }

        afterMap?.Invoke(value, result);
      }

      public bool TryMapProperty(EntityObjectProperty property, out EntityObjectProperty result)
      {
        propertyMappingsByFromProperty.TryGetValue(property, out var mapping);

        result = mapping?.ToProperty;

        return result != null;
      }
    }

    private class MappingProperty
    {
      public EntityObjectProperty FromProperty { get; }
      public EntityObjectProperty ToProperty { get; }
      public Func<IEntityObjectMapper, object, object, object> ValueSource { get; }

      public MappingProperty(EntityObjectProperty fromProperty, EntityObjectProperty toProperty, Func<IEntityObjectMapper, object, object, object> valueSource)
      {
        FromProperty = fromProperty;
        ToProperty = toProperty;
        ValueSource = valueSource;
      }
    }

    private class EntityObjectMapper : IEntityObjectMapper
    {
      private readonly Dictionary<(Type Source, Type Target), Mapper> mappers;
      private readonly EntityObjectMapperBuilder entityObjectMapperBuilder;

      public EntityObjectMapper(IEnumerable<Mapper> mappers, EntityObjectMapperBuilder entityObjectMapperBuilder)
      {
        this.mappers = mappers.ToDictionary(p => (p.FromType, p.ToType), p => p);
        this.entityObjectMapperBuilder = entityObjectMapperBuilder;
      }

      public T Map<T>(object value)
      {
        return (T)Map(value, typeof(T));
      }

      public object Map(object value, Type targetType)
      {
        return Map(value, null, targetType);
      }

      public void Map(object value, object target)
      {
        Map(value, target, target.GetType());
      }

      private object Map(object value, object target, Type targetType)
      {
        if (value == null)
          return null;

        var mapper = FindMapper(value.GetType(), targetType);
        
        if (mapper == null)
        {
          if (value.GetType() == targetType)
          {
            mapper = CreateMapper(value.GetType(), targetType);
          }
          else
            throw new ArgumentException($"No mapping exists from '{value.GetType()}' to '{targetType}'");
        }

        if (target == null)
          target = Activator.CreateInstance(targetType);

        mapper.Map(value, target, this);

        return target;
      }

      private Mapper CreateMapper(Type source, Type target)
      {
        var key = (source, target);
        var mapperBuilder = (IMapperBuilder)Activator.CreateInstance(typeof(MapperBuilder<,>).MakeGenericType(source, target));
        entityObjectMapperBuilder.builders.Add(key, mapperBuilder);
        var mapper = mapperBuilder.GetMapper(entityObjectMapperBuilder);
        mappers.Add(key, mapper);
        return mapper;
      }

      public IList<T> MapList<T>(IEnumerable values)
      {
        return MapList<T>(values, typeof(T));
      }

      public IList<T> MapList<T>(IEnumerable values, Type targetType)
      {
        var result = new List<T>();

        MapList(values, targetType, result);

        return result;
      }

      public IList MapList(IEnumerable values, Type targetType)
      {
        var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));

        MapList(values, targetType, result);

        return result;
      }

      public void MapList(IEnumerable values, Type targetType, IList target)
      {
        foreach (var value in values)
        {
          target.Add(Map(value, targetType));
        }
      }

      public void MapArray(Array values, Type targetType, Array target)
      {
        for (int i = 0; i < values.Length; i ++)
        {
          target.SetValue(Map(values.GetValue(i), targetType), i);
        }
      }

      public bool TryMapProperty(EntityObjectProperty property, Type targetType, out EntityObjectProperty result)
      {
        var mapper = FindMapper(property.EntityType, targetType);
        if (mapper != null)
          return mapper.TryMapProperty(property, out result);

        result = null;
        return false;
      }

      private Mapper FindMapper(Type sourceType, Type targetType)
      {
        for (; sourceType != null; sourceType = sourceType.BaseType)
        {
          if (mappers.TryGetValue((sourceType, targetType), out var mapper))
            return mapper;
        }

        return null;
      }
    }
  }
}
