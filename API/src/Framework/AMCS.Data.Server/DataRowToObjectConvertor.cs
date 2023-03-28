//-----------------------------------------------------------------------------
// <copyright file="DataRowToObjectConvertor.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AMCS.Data.Entity;
using AMCS.Data.Server.SQL;
using Microsoft.SqlServer.Types;
using NodaTime;

namespace AMCS.Data.Server
{
  [Obsolete("Use new ORM system")]
  public static class DataRowToObjectConvertor
  {
    private static readonly IList<string> EmptyFields = new string[0];

    /// <summary>
    /// Does the convert data row to data contract object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="accessor">The <see cref="EntityObjectAccessor"/> for the table.</param>
    /// <param name="addLoadedPropertyValues"></param>
    /// <returns></returns>
    internal static object DoConvertDataRowToDataContractObject(IDataReader row, EntityObjectAccessor accessor, bool addLoadedPropertyValues, string prefix = null)
    {
      var convertedObject = Activator.CreateInstance(accessor.Type);
      var entityObject = convertedObject as EntityObject;

      addLoadedPropertyValues = addLoadedPropertyValues || accessor.SupportsTracking;

      object[] loadedPropertyValues = null;
      if (addLoadedPropertyValues)
      {
        loadedPropertyValues = new object[accessor.Properties.Count];
      }

      List<(EntityObjectProperty Property, object Value)> delayedDateProperties = null;

      for (int fieldIndex = 0; fieldIndex < row.FieldCount; fieldIndex++)
      {
        string fieldName = row.GetName(fieldIndex);
        string propertyName = fieldName;
        if (prefix != null)
        {
          if (!fieldName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            continue;
          propertyName = fieldName.Substring(prefix.Length);
        }
        var property = accessor.GetPropertyByColumnName(propertyName);
        if (property != null)
        {
          object fieldValue;

          var fieldType = row.GetDataTypeName(fieldIndex).Split('.').Last();
          if (fieldType.ToLowerInvariant() == "geography")
          {
            fieldValue = SQLGeographyHelper.GetSqlGeographyData(row, fieldIndex);
          }
          else
            fieldValue = row[fieldName];

          object setPropValue = null;

          if (fieldValue != DBNull.Value)
          {
            try
            {
              var dateStorage = property.Column.DateStorage;

              if (dateStorage != DateStorage.None)
              {
                DateTimeZone timeZone = null;

                if (dateStorage == DateStorage.Zoned)
                {
                  if (entityObject == null)
                    throw new DateConversionException("Zoned date times can only be defined on EntityObject's");

                  if (TimeZoneUtils.TimeZoneMode == TimeZoneMode.NeutralEverywhere)
                  {
                    timeZone = TimeZoneUtils.NeutralTimeZone;
                  }
                  else
                  {
                    // Zoned date/times require a time zone. This is retrieved through a separate
                    // property on the entity object. However, that may not yet be initialized.
                    // As such, we delay processing these properties until the rest of the entity
                    // object is initialized.

                    if (delayedDateProperties == null)
                      delayedDateProperties = new List<(EntityObjectProperty, object)>();
                    delayedDateProperties.Add((property, fieldValue));

                    continue;
                  }
                }

                setPropValue = property.Column.DateStorageConverter.FromStorage(fieldValue, timeZone);
              }
              else if (fieldValue is SqlGeography sqlGeo)
              {
                if (property.Type.UnderlyingSystemType == typeof(GeographyPolygon))
                {
                  GeographyPolygon polygon = new GeographyPolygon();
                  polygon.Positions = new List<GeographyPoint>();
                  for (int i = 1; i <= sqlGeo.STNumPoints(); i++)
                  {
                    SqlGeography sqlPoint = sqlGeo.STPointN(i);
                    polygon.Positions.Add(new GeographyPoint { Lat = (double)sqlPoint.Lat, Long = (double)sqlPoint.Long });
                  }
                  setPropValue = polygon;
                }
                else
                {
                  var geo = new GeographyPoint { Lat = (double)sqlGeo.Lat, Long = (double)sqlGeo.Long };
                  setPropValue = geo;
                }
              }
              else
              {
                setPropValue = fieldValue;
                if (property.Type == typeof(char) && fieldValue != null)
                {
                  string stringValue = fieldValue.ToString();
                  if (stringValue.Length > 0)
                    setPropValue = stringValue[0];
                }
              }
            }
            catch (Exception e)
            {
              throw new Exception(string.Format("Field {0}: {1}", fieldName, e.Message), e);
            }
          }
          property.SetValue(convertedObject, setPropValue);

          var propertyIndex = accessor.Properties.IndexOf(property);
          if (addLoadedPropertyValues && propertyIndex != -1)
            loadedPropertyValues[propertyIndex] = setPropValue;
        }
      }

      if (delayedDateProperties != null)
      {
        foreach (var delayedDateProperty in delayedDateProperties)
        {
          var timeZoneMember = accessor.GetPropertyByColumnName(delayedDateProperty.Property.Column.TimeZoneMember);
          string timeZoneId = (string)timeZoneMember.GetValue(entityObject);
          if (timeZoneId == null)
            throw new DateConversionException("The time zone member does not contain a valid time zone");
          var timeZone = TimeZoneUtils.DateTimeZoneProvider[timeZoneId];

          object setPropValue = delayedDateProperty.Property.Column.DateStorageConverter.FromStorage(
            delayedDateProperty.Value,
            timeZone);

          delayedDateProperty.Property.SetValue(convertedObject, setPropValue);

          var propertyIndex = accessor.Properties.IndexOf(delayedDateProperty.Property);
          if (addLoadedPropertyValues && propertyIndex != -1)
            loadedPropertyValues[propertyIndex] = setPropValue;
        }
      }

      ((ILoadedPropertyValues)convertedObject).SetLoadedPropertyValues(loadedPropertyValues);

      return convertedObject;
    }

    /// <summary>
    /// Converts the data row to data contract object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="objectClassType">Type of the object class.</param>
    /// <param name="addLoadedProperties">Whether to set the loaded properties.</param>
    /// <returns></returns>
    public static object ConvertDataRowToDataContractObject(IDataReader row, Type objectClassType, bool addLoadedProperties)
    {
      var accessor = EntityObjectAccessor.ForType(objectClassType);
      return DoConvertDataRowToDataContractObject(row, accessor, addLoadedProperties);
    }

    /// <summary>
    /// Converts the data row to data contract object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="objectClassType">Type of the object class.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="addLoadedProperties">Whether to set the loaded properties.</param>
    /// <returns></returns>
    [Obsolete("Use overload without tableName")]
    public static object ConvertDataRowToDataContractObject(IDataReader row, Type objectClassType, string tableName, bool addLoadedProperties)
    {
      // TODO: This overload doesn't seem used, and breaks EntityObjectAccessor since it'd be
      // rerequesting the column mappings by a different table name. For now, this should be fine,
      // but ideally the overload would be removed.
      var accessor = EntityObjectAccessor.ForType(objectClassType);
      if (!string.IsNullOrEmpty(tableName) && accessor.TableName != tableName)
        throw new InvalidOperationException("Cannot pass non standard table name");
      return DoConvertDataRowToDataContractObject(row, accessor, addLoadedProperties);
    }

    /// <summary>
    /// Converts the data reader to data contract object list.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="objectClassType">Type of the object class.</param>
    /// <param name="list">The list.</param>
    /// <param name="addLoadedProperties">Whether to set the loaded properties.</param>
    /// <returns></returns>
    public static IList ConvertDataReaderToDataContractObjectList(IDataReader reader, Type objectClassType, IList list, bool addLoadedProperties)
    {
      var accessor = EntityObjectAccessor.ForType(objectClassType);

      while (reader.Read())
      {
        list.Add(DoConvertDataRowToDataContractObject(reader, accessor, addLoadedProperties));
      }

      return list;
    }

    /// <summary>
    /// Converts the data reader to data contract object list.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="objectClassType">Type of the object class.</param>
    /// <param name="list">The list.</param>
    /// <param name="childObjectType">Type of the child object.</param>
    /// <param name="childPropertyName">Name of the child property.</param>
    /// <param name="addLoadedProperties">Whether to set the loaded properties.</param>
    /// <returns></returns>
    public static IList ConvertDataReaderToDataContractObjectList(IDataReader reader, Type objectClassType, IList list, Type childObjectType, string childPropertyName, bool addLoadedProperties)
    {
      var accessor = EntityObjectAccessor.ForType(objectClassType);
      var childAccessor = EntityObjectAccessor.ForType(childObjectType);
      var childProperty = accessor.GetProperty(childPropertyName);

      while (reader.Read())
      {
        object nextObject = DoConvertDataRowToDataContractObject(reader, accessor, addLoadedProperties);
        object childObject = DoConvertDataRowToDataContractObject(reader, childAccessor, addLoadedProperties);
        childProperty.SetValue(nextObject, childObject);

        list.Add(nextObject);
      }

      return list;
    }

    /// <summary>
    /// Validates the data reader to data contract object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="objectClassType">Type of the object class.</param>
    /// <param name="fieldsNotMatched">The fields not matched.</param>
    /// <param name="attributesNotMatched">The attributes not matched.</param>
    /// <returns></returns>
    public static bool ValidateDataReaderToDataContractObject(IDataReader row, Type objectClassType, out string fieldsNotMatched, out string attributesNotMatched)
    {
      return ValidateDataReaderToDataContractObject(row, objectClassType, EmptyFields, out fieldsNotMatched, out attributesNotMatched);
    }

    /// <summary>
    /// Validates the data reader to data contract object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="objectClassType">Type of the object class.</param>
    /// <param name="ignore">The ignore.</param>
    /// <param name="fieldsNotMatched">The fields not matched.</param>
    /// <param name="attributesNotMatched">The attributes not matched.</param>
    /// <returns></returns>
    public static bool ValidateDataReaderToDataContractObject(IDataReader row, Type objectClassType, IList<string> ignore, out string fieldsNotMatched, out string attributesNotMatched)
    {
      var fieldsNotMatchedBuilder = new StringBuilder();
      var attributesNotMatchedBuilder = new StringBuilder();
      var fieldNames = new HashSet<string>();
      var accessor = EntityObjectAccessor.ForType(objectClassType);

      for (int fieldIndex = 0; fieldIndex < row.FieldCount; fieldIndex++)
      {
        string fieldName = row.GetName(fieldIndex);
        fieldNames.Add(fieldName);

        var property = accessor.GetPropertyByColumnName(fieldName);
        if (property == null && !ignore.Contains(fieldName))
        {
          fieldsNotMatchedBuilder.Append(fieldName);
        }
      }

      foreach (var property in accessor.Properties)
      {
        if (property.Column != null && !fieldNames.Contains(property.Column.ColumnName) && !ignore.Contains(property.Column.ColumnName))
        {
          attributesNotMatchedBuilder.Append(property.Name);
        }
      }

      fieldsNotMatched = fieldsNotMatchedBuilder.ToString();
      attributesNotMatched = attributesNotMatchedBuilder.ToString();

      return fieldsNotMatchedBuilder.Length == 0 && attributesNotMatchedBuilder.Length == 0;
    }
  }
}