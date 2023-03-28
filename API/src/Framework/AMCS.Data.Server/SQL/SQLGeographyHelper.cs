namespace AMCS.Data.Server.SQL
{
  using System.Data;
  using System.Data.SqlClient;
  using System.Data.SqlTypes;
  using Microsoft.SqlServer.Types;

  internal static class SQLGeographyHelper
  {
    public static object GetSqlGeographyData(IDataReader dataReader, int fieldIndex)
    {
      var sqlBytes = new SqlBytes();

      switch (dataReader)
      {
        case SqlDataReader sqlDataReader:
          sqlBytes = sqlDataReader.GetSqlBytes(fieldIndex);
          break;
        case DataReaderProxy dataReaderProxy:
          if (dataReaderProxy.Inner is SqlDataReader innerSqlDataReader)
            sqlBytes = innerSqlDataReader.GetSqlBytes(fieldIndex);
          break;
      }

      return !sqlBytes.IsNull ? SqlGeography.Deserialize(sqlBytes) : null;
    }
  }
}