using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AMCS.Data.Entity;
using log4net;

namespace AMCS.Data.Server.SQL
{
  internal class SQLUpdateCommandBuilder : SQLCommandBuilder<ISQLUpdate>, ISQLUpdate
  {
    private readonly ISessionToken userToken;
    private readonly EntityObject entityObject;

    private IList<string> specialFields;
    private bool ignoreSpecialFields;
    private IList<string> restrictToFields;
    private bool updateOverridableDynamicColumns;
    private bool createAuditRecord;
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SQLUpdateCommandBuilder));

    public SQLUpdateCommandBuilder(SQLDataSession dataSession, ISessionToken userToken, EntityObject entityObject)
      : base(dataSession)
    {
      this.userToken = userToken;
      this.entityObject = entityObject;
    }

    public ISQLUpdate CreateAuditRecord()
    {
      return CreateAuditRecord(true);
    }

    public ISQLUpdate CreateAuditRecord(bool value)
    {
      createAuditRecord = value;
      return this;
    }

    public ISQLUpdate IgnoreSpecialFields()
    {
      return IgnoreSpecialFields(true);
    }

    public ISQLUpdate IgnoreSpecialFields(bool value)
    {
      ignoreSpecialFields = value;
      return this;
    }

    public ISQLUpdate UpdateOverridableDynamicColumns()
    {
      return UpdateOverridableDynamicColumns(true);
    }

    public ISQLUpdate UpdateOverridableDynamicColumns(bool value)
    {
      updateOverridableDynamicColumns = value;
      return this;
    }

    public ISQLUpdate SpecialFields(IList<string> fields)
    {
      specialFields = fields;
      return this;
    }

    public ISQLUpdate RestrictToFields(IList<string> fields)
    {
      restrictToFields = fields;
      return this;
    }

    protected override ISQLCommandFactory CreateCommandFactory()
    {
      var preparedParameters = SQLPreparedParameters.ForUpdate(entityObject, updateOverridableDynamicColumns, specialFields, ignoreSpecialFields, restrictToFields);
      var parameters = preparedParameters.Parameters;

      if (parameters.Count == 0)
        return null; // there is nothing to changes with this object

      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());

      var sql = new SQLTextBuilder();

      sql.Text("UPDATE ").TableName(accessor).Text(" SET ");

      for (var i = 0; i < parameters.Count; i++)
      {
        if (i > 0)
          sql.Text(", ");

        sql
          .Name(parameters[i].ColumnName)
          .Text(" = ")
          .ParameterName(parameters[i].Parameter);
      }

      sql
        .Text(" WHERE ").Name(accessor.KeyName)
        .Text(" = ").ParameterName(preparedParameters.KeyParameter.Parameter);

      var sqlParameters = new List<SqlParameter>();

      sqlParameters.Add(preparedParameters.KeyParameter.Parameter);
      sqlParameters.AddRange(parameters.Select(p => p.Parameter));

      return new SQLCommandFactory(
        CommandType.Text,
        sql.ToString(),
        sqlParameters);
    }

    public void Execute()
    {
      DataSession.Events.BeforeUpdate(userToken, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, DataUpdateKind.Update);

      DataSession.Metrics?.UpdateBegin(DataSession.Connection);

      try
      {
        var result = ExecuteNonQuery();
        if (result.RowsAffected == 0)
        {
          string commandText = result.Command.CommandText;
          foreach (SqlParameter par in result.Command.Parameters)
          {
            commandText = commandText.Replace(par.ParameterName, result.Get(par.ParameterName).ToString());
          }
          Logger.Warn($"An update was performed that affected 0 rows; SQL Command {commandText};");
        }
        DataSession.Metrics?.UpdateEnd(DataSession.Connection, result.Command, result.RowsAffected);
      }
      catch (Exception ex)
      {
        DataSession.Metrics?.UpdateEnd(DataSession.Connection, null, 0, ex);
        throw;
      }

      DataSession.Events.AfterUpdate(userToken, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, DataUpdateKind.Update);

      if (createAuditRecord)
        SQLDataAccessAudit.InsertAuditRecord(userToken, entityObject.GetTableName(), entityObject.GetKeyName(), entityObject.Id32, SQLDataAuditChangeType.Update, DataSession);
    }
  }
}
