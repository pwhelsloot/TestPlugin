using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AMCS.Data.Support;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.SqlServer.Types;

namespace AMCS.Data.Schema.Access.SqlServer
{
  /// <summary>
  /// An SQL Server specific implementation of IDatabaseInterface.
  /// </summary>
  public class SqlServerInterface : IDatabaseInterface
  {
    public string ConnectionString { get; private set; }
    public IsolationLevel TransactionIsolationLevel { get; set; }

    private int _commandTimeout = -1;

    public SqlServerInterface(string connectionString, int commandTimeout = 30)
    {
      if (connectionString == null)
        throw new ArgumentNullException(nameof(connectionString));
      ConnectionString = connectionString;
      _commandTimeout = commandTimeout;
      TransactionIsolationLevel = IsolationLevel.ReadCommitted;
    }

    public int ExecuteNonQuery(string sql, IDictionary<string, object> parameters = null)
    {
      try
      {
        using (var connection = ConnectionStringUtils.OpenSqlConnection(ConnectionString))
        {
          //using (SqlTransaction tran = conn.BeginTransaction(TransactionIsolationLevel))
          //{
          try
          {
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
              //cmd.Transaction = tran;
              if (_commandTimeout >= 0)
                cmd.CommandTimeout = _commandTimeout;

              if (parameters != null && parameters.Count > 0)
              {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                  object paramValue = parameter.Value;
                  if (paramValue == null)
                    paramValue = DBNull.Value;
                  SqlParameter p = new SqlParameter(parameter.Key, parameter.Value);

                  if (paramValue is SqlGeography)
                    p.UdtTypeName = "Geography";

                  cmd.Parameters.Add(p);
                }
              }

              int result = cmd.ExecuteNonQuery();
              // tran.Commit();
              return result;
            }
          }
          catch
          {
            //if (tran != null)
            //  tran.Rollback();
            throw;
          }
          //}
        }
      }
      catch (Exception ex)
      {
        throw new DataAccessException(string.Format("{0} - occurred when executing statement:\r\n{1}\r\n", ex.Message, sql), ex);
      }
    }

    public IDataReader ExecuteReader(string sql, IDictionary<string, object> parameters = null)
    {
      try
      {
        using (var connection = ConnectionStringUtils.OpenSqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, connection))
        {
          if (_commandTimeout >= 0)
            cmd.CommandTimeout = _commandTimeout;

          if (parameters != null && parameters.Count > 0)
          {
            foreach (KeyValuePair<string, object> parameter in parameters)
              cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
          }
          return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
      }
      catch (Exception ex)
      {
        throw new DataAccessException(string.Format("{0} - occurred when executing statement:\r\n{1}\r\n", ex.Message, sql), ex);
      }
    }

    public DataTable GetDataTable(string sql, IDictionary<string, object> parameters = null)
    {
      return GetDataTable(sql, parameters, false);
    }

    public DataTable GetDataTable(string sql, IDictionary<string, object> parameters, bool executeWithinNewTransaction)
    {
      try
      {
        using (var connection = ConnectionStringUtils.OpenSqlConnection(ConnectionString))
        { 
          SqlTransaction tran = null;
          if (executeWithinNewTransaction)
            tran = connection.BeginTransaction(TransactionIsolationLevel);

          try
          {
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
              if (tran != null)
                cmd.Transaction = tran;
              if (_commandTimeout >= 0)
                cmd.CommandTimeout = _commandTimeout;

              if (parameters != null && parameters.Count > 0)
              {
                foreach (KeyValuePair<string, object> parameter in parameters)
                  cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
              }
              //if DataTables are going to be sent over WCF they need a table name
              DataTable dataTable = new DataTable("DefaultTable");
              dataTable.Load(cmd.ExecuteReader());

              if (tran != null)
                tran.Commit();

              return dataTable;
            }
          }
          catch
          {
            if (tran != null)
              tran.Rollback();
            throw;
          }
        }
      }
      catch (Exception ex)
      {
        throw new DataAccessException(string.Format("{0} - occurred when executing statement:\r\n{1}\r\n", ex.Message, sql), ex);
      }
    }
  }
}
