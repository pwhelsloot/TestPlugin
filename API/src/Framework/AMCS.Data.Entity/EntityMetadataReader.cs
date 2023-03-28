using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public static class EntityMetadataReader
  {
    public static bool GetTrackInserts(Type type)
    {
      return GetTableAttribute(type)?.TrackInserts ?? false;
    }

    public static bool GetTrackUpdates(Type type)
    {
      return GetTableAttribute(type)?.TrackUpdates ?? false;
    }

    public static bool GetTrackDeletes(Type type)
    {
      return GetTableAttribute(type)?.TrackDeletes ?? false;
    }

    public static string GetSchemaName(Type type)
    {
      return GetTableAttribute(type)?.SchemaName ?? "dbo";
    }

    public static string GetTableName(Type type)
    {
      return GetTableAttribute(type)?.TableName ?? string.Empty;
    }

    public static string GetObjectName(Type type)
    {
      return GetTableAttribute(type)?.ObjectName;
    }

    public static string GetKeyField(Type type)
    {
      return GetTableAttribute(type)?.KeyField ?? string.Empty;
    }
    
    public static IdentityInsertMode GetIdentityInsertMode(Type type)
    {
      return GetTableAttribute(type)?.IdentityInsertMode ?? IdentityInsertMode.Off;
    }
    
    public static bool GetInsertOnNullId(Type type)
    {
      return GetTableAttribute(type)?.InsertOnNullId ?? false;
    }

    private static EntityTableAttribute GetTableAttribute(Type type)
    {
      return type.GetCustomAttribute<EntityTableAttribute>(true);
    }

    public static bool IsMember(PropertyInfo property)
    {
      var dataMemberAttribute = property.GetCustomAttribute<DataMemberAttribute>();
      if (dataMemberAttribute != null)
        return !string.IsNullOrEmpty(dataMemberAttribute.Name);
      return property.GetCustomAttribute<EntityMemberAttribute>() != null;
    }

    public static string GetMemberColumnName(PropertyInfo property)
    {
      var dataMemberAttribute = property.GetCustomAttribute<DataMemberAttribute>();
      if (dataMemberAttribute != null)
        return dataMemberAttribute.Name;
      var entityMemberAttribute = property.GetCustomAttribute<EntityMemberAttribute>();
      if (entityMemberAttribute != null)
        return entityMemberAttribute.Name ?? property.Name;
      return null;
    }

    public static bool IsMemberDynamic(PropertyInfo property)
    {
      var dynamicColumnAttribute = property.GetCustomAttribute<DynamicColumn>();
      if (dynamicColumnAttribute != null)
        return true;
      var entityMemberAttribute = property.GetCustomAttribute<EntityMemberAttribute>();
      if (entityMemberAttribute != null)
        return entityMemberAttribute.IsDynamic;
      return false;
    }

    public static string GetMemberDynamicName(PropertyInfo property)
    {
      return property.GetCustomAttribute<DynamicColumn>()?.Name;
    }

    public static bool IsMemberOverridable(PropertyInfo property)
    {
      var dynamicColumnAttribute = property.GetCustomAttribute<DynamicColumn>();
      if (dynamicColumnAttribute != null)
        return dynamicColumnAttribute.IsOverridable;
      var entityMemberAttribute = property.GetCustomAttribute<EntityMemberAttribute>();
      if (entityMemberAttribute != null)
        return entityMemberAttribute.IsOverridable;
      return false;
    }

    public static DateStorage GetDateStorage(PropertyInfo property)
    {
      var entityMemberAttribute = property.GetCustomAttribute<EntityMemberAttribute>();
      if (entityMemberAttribute != null)
        return entityMemberAttribute.DateStorage;
      return DateStorage.None;
    }

    public static string GetTimeZoneMember(PropertyInfo property)
    {
      var entityMemberAttribute = property.GetCustomAttribute<EntityMemberAttribute>();
      return entityMemberAttribute?.TimeZoneMember;
    }

    public static ApiPropertyConfiguration GetApiPropertyConfiguration(PropertyInfo property)
    {
      var attribute =
        property.GetCustomAttribute<ApiPropertyAttribute>() ??
        new ApiPropertyAttribute();

      return ApiPropertyConfiguration.FromAttribute(attribute);
    }
  }
}
