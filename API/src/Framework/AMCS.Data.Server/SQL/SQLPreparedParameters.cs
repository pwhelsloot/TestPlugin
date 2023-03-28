using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.Services;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.Globalization;
using NodaTime;

namespace AMCS.Data.Server.SQL
{
  internal class SQLPreparedParameters
  {
    public static SQLPreparedParameters ForInsert(EntityObject entityObject, bool identityInsert, IList<string> restrictToFields, bool insertOverridableDynamicColumns)
    {
      var parameters = new List<SQLPreparedParameter>();
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());
      var parameterSet = ParameterSet.Build(accessor, identityInsert, null, false, restrictToFields, insertOverridableDynamicColumns);

      // BLOB fields may modify associated hash fields. Because of this, we first
      // need to process all BLOB fields before we process the rest.

      foreach (var property in parameterSet.BlobProperties)
      {
        var parameter = CreateBlobParameter(property, entityObject, accessor);
        if (parameter != null)
          parameters.Add(parameter);
      }

      foreach (var property in parameterSet.Properties)
      {
        string parameterName = "@" + property.Column.ColumnName;
        object value = property.GetValue(entityObject);

        if (value == null && property.Name == "GUID")
          value = SQLGuidComb.Generate();

        if (value != null)
          value = ConvertDateValue(accessor, entityObject, property, value);

        parameters.Add(CreateParameter(property, parameterName, value));
      }

      return new SQLPreparedParameters(null, parameters);
    }

    public static SQLPreparedParameters ForUpdate(EntityObject entityObject, bool updateOverridableDynamicColumns, IList<string> specialFields, bool ignoreSpecialFields, IList<string> restrictToFields)
    {
      var parameters = new List<SQLPreparedParameter>();
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());
      var parameterSet = ParameterSet.Build(accessor, false, specialFields, ignoreSpecialFields, restrictToFields, updateOverridableDynamicColumns);

      SQLPreparedParameter keyParameter = null;
      if (parameterSet.KeyProperty != null)
        keyParameter = CreateParameter(parameterSet.KeyProperty, "@keyValue", parameterSet.KeyProperty.GetValue(entityObject));

      // BLOB fields may modify associated hash fields. Because of this, we first
      // need to process all BLOB fields before we process the rest.

      foreach (var property in parameterSet.BlobProperties)
      {
        var parameter = CreateBlobParameter(property, entityObject, accessor);
        if (parameter != null)
          parameters.Add(parameter);
      }

      foreach (var property in parameterSet.Properties)
      {
        // Ignore GUID column for all updates - should never be changed (following chat with Tony) - Seb
        if (property.Name == "GUID")
          continue;

        string parameterName = "@" + property.Name;
        object value = property.GetValue(entityObject);

        if (value != null)
          value = ConvertDateValue(accessor, entityObject, property, value);

        parameters.Add(CreateParameter(property, parameterName, value));
      }

      return new SQLPreparedParameters(keyParameter, parameters);
    }

    private static SQLPreparedParameter CreateBlobParameter(EntityObjectProperty property, EntityObject entityObject, EntityObjectAccessor accessor)
    {
      var blob = (EntityBlob)property.GetValue(entityObject);

      // We enforce the invariant here that if there is a hash, the BLOB field MUST be NULL.
      // That's why we add the null parameter below both when we've been asked explicitly
      // to clear the BLOB, or when the entity has a hash set.
      //
      // We proactively take out the pending content here to ensure it's not reused a second
      // time of the same entity is saved again.

      if (blob.TryGetPendingBlob(out var buffer, remove: true))
      {
        // Entities saved through a WCF service will always come in as pending BLOBs.
        // Because of this, we do need to check here whether blob storage is external and,
        // if so, save it to the external system.

        if (buffer != null)
        {
          var service = DataServices.Resolve<IBlobStorageService>();

          if (service.ExternalStorage)
          {
            using (var stream = new MemoryStream(buffer))
            {
              string hash = service.SaveBlob(stream);

              accessor.GetProperty(blob.Metadata.HashMemberName).SetValue(entityObject, hash);
            }

            // Set the buffer to null so that we still clear the BLOB field.

            buffer = null;
          }
        }

        return CreateParameter(typeof(byte[]), blob.Metadata.BlobColumnName, "@" + blob.Metadata.BlobColumnName, buffer);
      }

      // Clear the blob field if we have a hash.

      string existingHash = (string)accessor.GetProperty(blob.Metadata.HashMemberName).GetValue(entityObject);
      if (!string.IsNullOrEmpty(existingHash))
        return CreateParameter(typeof(byte[]), blob.Metadata.BlobColumnName, "@" + blob.Metadata.BlobColumnName, null);

      return null;
    }

    private static SQLPreparedParameter CreateParameter(EntityObjectProperty property, string name, object value)
    {
      return CreateParameter(property.Type, property.Column.ColumnName, name, value);
    }

    private static SQLPreparedParameter CreateParameter(Type propertyType, string columnName, string name, object value)
    {
      SqlParameter parameter;

      if (value == null)
        value = DBNull.Value;

      if (propertyType == typeof(GeographyPoint))
      {
        parameter = BuildSqlGeographyPointParameter(name, value);
      }
      else if (propertyType == typeof(GeographyPolygon))
      {
        parameter = BuildSqlGeographyPolygonParameter(name, value);
      }
      else if (propertyType == typeof(byte[]))
      {
        // If byte[] object is null, it is incorrectly cast to a varchar sql db type. This fix prevents this and was tested to work with varbinary, image and binary column types.
        parameter = new SqlParameter(name, value) { SqlDbType = SqlDbType.VarBinary };
      }
      else
      {
        parameter = new SqlParameter(name, value);
      }

      return new SQLPreparedParameter(columnName, parameter);
    }

    private static SqlParameter BuildSqlGeographyPointParameter(string name, object value)
    {
      if (value is GeographyPoint geo && geo.Lat != null && geo.Long != null)
      {
        var sqlGeo = SqlGeography.Point(geo.Lat.Value, geo.Long.Value, 4326);

        return new SqlParameter(name, sqlGeo) { UdtTypeName = "Geography" };
      }

      return new SqlParameter(name, DBNull.Value);
    }

    private static SqlParameter BuildSqlGeographyPolygonParameter(string name, object value)
    {
      if (value is GeographyPolygon poly && poly.Positions != null && poly.Positions.Count > 0)
      {
        var coordinates = OrderCoordinates(poly.Positions);

        StringBuilder stringBuilder = new StringBuilder("POLYGON((");
        for (int i = 0; i < coordinates.Count; i++)
        {
          stringBuilder.Append(coordinates[i].Long.Value.ToString(CultureInfo.InvariantCulture));
          stringBuilder.Append(" ");
          stringBuilder.Append(coordinates[i].Lat.Value.ToString(CultureInfo.InvariantCulture));
          if (i != (coordinates.Count - 1))
          {
            stringBuilder.Append(", ");
          }
          else
          {
            stringBuilder.Append("))");
          }
        }

        var sqlPoly = SqlGeography.Parse(new SqlString(stringBuilder.ToString()));

        return new SqlParameter(name, sqlPoly) { UdtTypeName = "Geography" };
      }

      return new SqlParameter(name, DBNull.Value);
    }

    private static List<GeographyPoint> OrderCoordinates(IList<GeographyPoint> geographyPoints)
    {
      double sumOfEdges = 0;

      for (int i = 0; i < geographyPoints.Count; i++)
      {
        if (i != geographyPoints.Count - 1)
        {
          sumOfEdges += (geographyPoints[i + 1].Lat.Value - geographyPoints[i].Lat.Value) * (geographyPoints[i + 1].Long.Value + geographyPoints[i].Long.Value);
        }
        else
        {
          sumOfEdges += (geographyPoints[0].Lat.Value - geographyPoints[i].Lat.Value) * (geographyPoints[0].Long.Value + geographyPoints[i].Long.Value);
        }
      }

      return sumOfEdges > 0 ? geographyPoints.Reverse().ToList() : geographyPoints.ToList();
    }

    private static object ConvertDateValue(EntityObjectAccessor accessor, EntityObject entityObject, EntityObjectProperty property, object value)
    {
      var dateStorageConverter = property.Column.DateStorageConverter;
      if (dateStorageConverter == null)
        return value;

      DateTimeZone timeZone = null;

      if (property.Column.TimeZoneMember != null)
      {
        if (TimeZoneUtils.TimeZoneMode == TimeZoneMode.NeutralEverywhere)
        {
          timeZone = TimeZoneUtils.NeutralTimeZone;
        }
        else
        {
          var timeZoneMember = accessor.GetPropertyByColumnName(property.Column.TimeZoneMember);
          string timeZoneId = (string)timeZoneMember.GetValue(entityObject);
          if (timeZoneId == null)
            throw new DateConversionException("The time zone member does not contain a valid time zone");
          timeZone = TimeZoneUtils.DateTimeZoneProvider[timeZoneId];
        }
      }

      return dateStorageConverter.ToStorage(value, timeZone);
    }

    public SQLPreparedParameter KeyParameter { get; }

    public IList<SQLPreparedParameter> Parameters { get; }

    private SQLPreparedParameters(SQLPreparedParameter keyParameter, IList<SQLPreparedParameter> parameters)
    {
      KeyParameter = keyParameter;
      Parameters = parameters;
    }

    private class ParameterSet
    {
      public static ParameterSet Build(EntityObjectAccessor accessor, bool identityInsert, IList<string> specialFields, bool ignoreSpecialFields, IList<string> restrictToFields, bool overridableDynamicColumns)
      {
        EntityObjectProperty keyProperty = null;
        var properties = new List<EntityObjectProperty>();
        var blobProperties = new List<EntityObjectProperty>();

        foreach (var property in accessor.Properties)
        {
          bool isBlob = property.Type == typeof(EntityBlob);

          if (!isBlob && property.Column?.CanWrite != true)
            continue;

          // Ignore dynamic properties that aren't overriden.
          if (property.IsDynamicColumn && (!overridableDynamicColumns || !property.IsDynamicColumnOverridable))
            continue;

          // Is this the key field? if so don't want to try and insert it.
          // TODO: This was wrong. This had 'entityObject.GetKeyName().Equals("Id")' where it now says 'property.Name == "Id"'.
          // So, the check was to test whether the key name of the object was "Id", where we now check whether the current
          // property is named "Id".
          if (!identityInsert && (property.IsKey || property.Name == "Id"))
          {
            keyProperty = property;
            continue;
          }

          // We don't want to ignore special fields (which means we want to add them) this field
          // is not special so don't carry on with it.
          if (specialFields != null && specialFields.Count > 0)
          {
            bool thisIsSpecialField = specialFields.Contains(property.Name);
            if (ignoreSpecialFields == thisIsSpecialField)
              continue;
          }

          // Got this far, so want to insert/update this property into the database.
          if (restrictToFields != null)
          {
            if (!restrictToFields.Contains(property.Name))
              continue;
            if (property.Column != null && !restrictToFields.Contains(property.Column.ColumnName))
              continue;
          }

          if (isBlob)
            blobProperties.Add(property);
          else
            properties.Add(property);
        }

        return new ParameterSet(keyProperty, properties, blobProperties);
      }

      public EntityObjectProperty KeyProperty { get; }

      public List<EntityObjectProperty> Properties { get; }

      public List<EntityObjectProperty> BlobProperties { get; }

      private ParameterSet(EntityObjectProperty keyProperty, List<EntityObjectProperty> properties, List<EntityObjectProperty> blobProperties)
      {
        KeyProperty = keyProperty;
        Properties = properties;
        BlobProperties = blobProperties;
      }
    }
  }
}
