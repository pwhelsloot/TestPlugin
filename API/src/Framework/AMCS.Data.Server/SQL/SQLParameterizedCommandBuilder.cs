#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.SQL.Fetch;
using AMCS.Data.Server.Util;
using Microsoft.SqlServer.Types;

namespace AMCS.Data.Server.SQL
{
  internal abstract class SQLParameterizedCommandBuilder<T> : SQLCommandBuilder<T>, ISQLParameterizedExecutable<T>
    where T : ISQLParameterizedExecutable<T>
  {
    private List<Parameter> parameters;
    private object arguments;

    protected SQLParameterizedCommandBuilder(SQLDataSession dataSession, FetchInfo fetchInfo)
      : base(dataSession, fetchInfo)
    {
    }

    protected IList<SqlParameter> CreateParameters()
    {
      var sqlParameters = new List<SqlParameter>();

      if (arguments != null)
        SQLCommandParameterBuilder.ForType(arguments.GetType()).Build(sqlParameters, arguments);

      if (parameters != null)
      {
        foreach (var parameter in parameters)
        {
          sqlParameters.Add(parameter.CreateParameter());
        }
      }

      return sqlParameters;
    }

    private T SetParameter(string parameterName, object value, SqlDbType? type, ParameterDirection? direction = null, int? size = null, int? precision = null, int? scale = null, string typeName = null)
    {
      if (parameters == null)
        parameters = new List<Parameter>();

      var parameter = new Parameter(parameterName, value, type, direction, size, precision, scale, typeName);

      for (int i = 0; i < parameters.Count; i++)
      {
        if (parameters[i].Name == parameterName)
        {
          parameters[i] = parameter;
          return Self();
        }
      }

      parameters.Add(parameter);

      return Self();
    }

    private T Self()
    {
      return (T)(ISQLParameterizedExecutable<T>)this;
    }

    public T Arguments(object arguments)
    {
      this.arguments = arguments;
      return Self();
    }

    public T Set(string parameterName, bool value)
    {
      return SetParameter(parameterName, value, SqlDbType.Bit);
    }

    public T Set(string parameterName, bool? value)
    {
      return SetParameter(parameterName, value, SqlDbType.Bit);
    }

    public T Set(string parameterName, int value)
    {
      return SetParameter(parameterName, value, SqlDbType.Int);
    }

    public T Set(string parameterName, int? value)
    {
      return SetParameter(parameterName, value, SqlDbType.Int);
    }

    public T Set(string parameterName, decimal value)
    {
      return SetParameter(parameterName, value, SqlDbType.Decimal);
    }

    public T Set(string parameterName, decimal value, int precision, int scale)
    {
      return SetParameter(parameterName, value, SqlDbType.Decimal, precision: precision, scale: scale);
    }

    public T Set(string parameterName, decimal? value)
    {
      return SetParameter(parameterName, value, SqlDbType.Decimal);
    }

    public T Set(string parameterName, decimal? value, int precision, int scale)
    {
      return SetParameter(parameterName, value, SqlDbType.Decimal, precision: precision, scale: scale);
    }

    public T Set(string parameterName, float value)
    {
      return SetParameter(parameterName, value, SqlDbType.Float);
    }

    public T Set(string parameterName, float value, int precision, int scale)
    {
      return SetParameter(parameterName, value, SqlDbType.Float, precision: precision, scale: scale);
    }

    public T Set(string parameterName, float? value)
    {
      return SetParameter(parameterName, value, SqlDbType.Float);
    }

    public T Set(string parameterName, float? value, int precision, int scale)
    {
      return SetParameter(parameterName, value, SqlDbType.Float, precision: precision, scale: scale);
    }

    public T Set(string parameterName, double value)
    {
      return SetParameter(parameterName, value, SqlDbType.Float);
    }

    public T Set(string parameterName, double value, int precision, int scale)
    {
      return SetParameter(parameterName, value, SqlDbType.Float, precision: precision, scale: scale);
    }

    public T Set(string parameterName, double? value)
    {
      return SetParameter(parameterName, value, SqlDbType.Float);
    }

    public T Set(string parameterName, double? value, int precision, int scale)
    {
      return SetParameter(parameterName, value, SqlDbType.Float, precision: precision, scale: scale);
    }

    public T Set(string parameterName, Guid value)
    {
      return SetParameter(parameterName, value, SqlDbType.UniqueIdentifier);
    }

    public T Set(string parameterName, Guid? value)
    {
      return SetParameter(parameterName, value, SqlDbType.UniqueIdentifier);
    }

    public T SetDate(string parameterName, DateTime value)
    {
      return SetParameter(parameterName, value, SqlDbType.Date);
    }

    public T SetDate(string parameterName, DateTime? value)
    {
      return SetParameter(parameterName, value, SqlDbType.Date);
    }

    public T Set(string parameterName, DateTime value)
    {
      return SetParameter(parameterName, value, SqlDbType.DateTime);
    }

    public T Set(string parameterName, DateTime? value)
    {
      return SetParameter(parameterName, value, SqlDbType.DateTime);
    }

    public T Set(string parameterName, DateTimeOffset value)
    {
      return SetParameter(parameterName, value, SqlDbType.DateTimeOffset);
    }

    public T Set(string parameterName, DateTimeOffset? value)
    {
      return SetParameter(parameterName, value, SqlDbType.DateTimeOffset);
    }

    public T Set(string parameterName, string value)
    {
      return SetParameter(parameterName, value, SqlDbType.NVarChar);
    }

    public T Set(string parameterName, byte[] value)
    {
      return SetParameter(parameterName, value, SqlDbType.VarBinary);
    }

    public T SetIdList(string parameterName, string listName, IEnumerable<int> values)
    {
      return SetIdList(parameterName, listName, values, null);
    }

    public T SetIdList(string parameterName, string listName, IEnumerable<int> values, string typeName)
    {
      List<int> list = null;
      if (values != null)
        list = values.ToList();
      return SetParameter(parameterName, SQLDataAccessHelper.ConvertIdListToTableType(listName, list), SqlDbType.Structured, typeName: typeName);
    }

    public T SetIdList(string parameterName, string listName, IEnumerable<Guid> values)
    {
      return SetIdList(parameterName, listName, values, null);
    }

    public T SetIdList(string parameterName, string listName, IEnumerable<Guid> values, string typeName)
    {
      List<Guid> list = null;
      if (values != null)
        list = values.ToList();
      return SetParameter(parameterName, SQLDataAccessHelper.ConvertIdListToTableType(listName, list), SqlDbType.Structured, typeName: typeName);
    }

    public T SetStringList(string parameterName, string listName, IEnumerable<string> values)
    {
      return SetStringList(parameterName, listName, values, null);
    }

    public T SetStringList(string parameterName, string listName, IEnumerable<string> values, string typeName)
    {
      List<string> list = null;
      if (values != null)
        list = values.ToList();
      return SetParameter(parameterName, SQLDataAccessHelper.ConvertStringListToTableType(listName, list), SqlDbType.Structured, typeName: typeName);
    }

    public T SetIdDataTable(string parameterName, IEnumerable<int> values)
    {
      return SetParameter(parameterName, DataHelper.IdsToDataTable(values), SqlDbType.Structured);
    }

    public T SetObject(string parameterName, object value)
    {
      return SetParameter(parameterName, value, null);
    }

    public T SetObject(string parameterName, object value, SqlDbType type)
    {
      return SetParameter(parameterName, value, type);
    }

    public T SetObject(string parameterName, object value, SqlDbType type, string typeName)
    {
      return SetParameter(parameterName, value, type, typeName: typeName);
    }

    public T SetOutput(string parameterName, SqlDbType type)
    {
      return SetParameter(parameterName, null, type, ParameterDirection.Output);
    }

    public T SetOutput(string parameterName, SqlDbType type, int precision, int scale)
    {
      return SetParameter(parameterName, null, type, ParameterDirection.Output, precision: precision, scale: scale);
    }

    public T SetReturnValue(string parameterName, SqlDbType type)
    {
      return SetParameter(parameterName, null, type, ParameterDirection.ReturnValue);
    }

    public T SetReturnValue(string parameterName, SqlDbType type, int precision, int scale)
    {
      return SetParameter(parameterName, null, type, ParameterDirection.ReturnValue, precision: precision, scale: scale);
    }

    private class Parameter
    {
      private readonly object value;
      private readonly SqlDbType? type;
      private readonly ParameterDirection? direction;
      private readonly int? size;
      private readonly int? precision;
      private readonly int? scale;
      private readonly string typeName;

      public string Name { get; }

      public Parameter(string name, object value, SqlDbType? type, ParameterDirection? direction, int? size, int? precision, int? scale, string typeName)
      {
        Name = name;
        this.value = value;
        this.type = type;
        this.direction = direction;
        this.size = size;
        this.precision = precision;
        this.scale = scale;
        this.typeName = typeName;
      }

      public SqlParameter CreateParameter()
      {
        object value = this.value;
        if (value == null && type != SqlDbType.Structured)
          value = DBNull.Value;

        SqlParameter parameter;

        if (type.HasValue)
          parameter = new SqlParameter(Name, type.Value) { Value = value };
        else
          parameter = new SqlParameter(Name, value);

        if (direction.HasValue)
          parameter.Direction = direction.Value;
        if (size.HasValue)
          parameter.Size = size.Value;
        else if (RequiresSize())
          parameter.Size = -1;
        if (precision.HasValue)
          parameter.Precision = (byte)precision.Value;
        if (scale.HasValue)
          parameter.Scale = (byte)scale.Value;
        if (typeName != null)
          parameter.TypeName = typeName;

        if (value is SqlGeography)
          parameter.UdtTypeName = "Geography";

        return parameter;
      }

      private bool RequiresSize()
      {
        // If we don't know, we don't attempt.
        if (!type.HasValue || !direction.HasValue)
          return false;

        // These conditions are taken from the SqlParameter reference source, specifically:

        // // NOTE: (General Criteria): SqlParameter does a Size Validation check and would fail if the size is 0. 
        // //                           This condition filters all scenarios where we view a valid size 0.
        // if (ADP.IsDirection(this, ParameterDirection.Output) &&
        //     !ADP.IsDirection(this, ParameterDirection.ReturnValue) && // SQL BU DT 372370
        //     (!metaType.IsFixed) && 
        //     !ShouldSerializeSize() && 
        //     ((null == _value) || Convert.IsDBNull(_value)) && 
        //     (SqlDbType != SqlDbType.Timestamp) && 
        //     (SqlDbType != SqlDbType.Udt) &&
        //     // 
        // 
        //     (SqlDbType != SqlDbType.Xml) &&
        //     !metaType.IsVarTime) {

        // Input only parameters don't need a length.
        if (
          direction.Value == ParameterDirection.Input ||
          direction.Value == ParameterDirection.ReturnValue)
          return false;
        if (size.GetValueOrDefault() != 0)
          return false;
        if (value != null)
          return false;
        switch (type.Value)
        {
          case SqlDbType.Char:
          case SqlDbType.NChar:
          case SqlDbType.VarChar:
          case SqlDbType.NVarChar:
          case SqlDbType.VarBinary:
            return true;
          default:
            return false;
        }
      }
    }
  }
}
