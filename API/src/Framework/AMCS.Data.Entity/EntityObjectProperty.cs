using System;
using System.Collections;
using System.Diagnostics;
using AMCS.Data.Support;

namespace AMCS.Data.Entity
{
  [DebuggerDisplay("Name = {Name}, ColumnName = {Column.ColumnName}")]
  public class EntityObjectProperty
  {
    private readonly Func<object, object> getValue;
    private readonly Action<object, object> setValue;
    private readonly Func<object> defaultValue;

    public string Name { get; }

    public Type Type { get; }

    public Type EntityType { get; }

    public EntityObjectColumn Column { get; }

    internal IEntityObjectReferenceMetadata Reference { get; }

    public bool IsKey { get; }

    public bool IsDatabaseColumn => Column?.CanWrite == true;

    public bool IsDynamicColumn { get; }

    public bool IsDynamicColumnOverridable { get; }

    public string DynamicColumnName { get; }

    public ApiPropertyConfiguration ApiConfiguration { get; }

    public bool CanRead { get; }

    public bool CanWrite { get; }

    public bool IsNullable { get; }

    internal EntityObjectProperty(Type entityType, string name, Type type, EntityObjectColumn column, IEntityObjectReferenceMetadata reference,
      bool isKey, bool isDynamicColumn, bool isDynamicColumnOverridable, string dynamicColumnName, ApiPropertyConfiguration apiConfiguration)
    {
      Name = name;
      Type = type;
      EntityType = entityType;
      Column = column;
      Reference = reference;
      IsKey = isKey;
      IsDynamicColumn = isDynamicColumn;
      IsDynamicColumnOverridable = isDynamicColumnOverridable;
      DynamicColumnName = dynamicColumnName;
      ApiConfiguration = apiConfiguration;

      var propertyInfo = entityType.GetProperty(name);

      CanRead = propertyInfo.CanRead;
      CanWrite = propertyInfo.CanWrite;

      getValue = ReflectionHelper.GetPropertyGetter(propertyInfo);
      setValue = ReflectionHelper.GetEntityPropertySetter(propertyInfo);
      IsNullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
      if (!IsNullable)
        defaultValue = ReflectionHelper.GetDefaultValueFactory(type);
    }

    public object GetValue(object target)
    {
      return getValue(target);
    }

    public void SetValue(object target, object value)
    {
      setValue(target, value);
    }

    public object GetDefaultValue()
    {
      return defaultValue?.Invoke();
    }

    public bool UseInTemplateQuery(object value, bool includeBooleanIfDefault = false)
    {
      // Only use property if
      // is not null,
      // not equal to default value (unless is a boolean that is explicitly defined in includeOnlyPropertyNames),
      // not an empty IEnumerable
      // & not a DynamicColumn

      if (value == null)
        return false;
      if (IsDynamicColumn)
        return false;
      if (value.Equals(GetDefaultValue()))
      {
        if (includeBooleanIfDefault && Type == typeof(bool))
          return true;
        return false;
      }
      if (value is ICollection collection && collection.Count == 0)
        return false;

      return true;
    }
  }
}
