using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.Services;

namespace AMCS.Data.Server.SQL
{
  public class SQLBlobMigrator
  {
    private readonly ISessionToken userId;
    private readonly int batchSize;
    private readonly int parallel;
    private readonly Action<string> log;
    private readonly CancellationToken cancellationToken;
    private readonly IBlobStorageService service = DataServices.Resolve<IBlobStorageService>();

    public SQLBlobMigrator(ISessionToken userId, int batchSize, int parallel, CancellationToken cancellationToken, Action<string> log)
    {
      this.userId = userId;
      this.batchSize = batchSize;
      this.parallel = parallel;
      this.cancellationToken = cancellationToken;
      this.log = log;
    }

    public void Migrate()
    {
      if (!service.ExternalStorage)
        throw new InvalidOperationException("No external Blob Storage has been configured");

      log("Finding entities");

      var tables = FindTables();

      log($"Migrating {tables.Count} tables");

      foreach (var table in tables)
      {
        MigrateTable(table);
      }
    }

    public static List<Table> FindTables()
    {
      var tables = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);

      foreach (var entity in DataServices.Resolve<EntityObjectManager>().Entities)
      {
        var accessor = EntityObjectAccessor.ForType(entity.Type);

        foreach (var property in accessor.Properties)
        {
          if (property.Type == typeof(EntityBlob))
          {
            EntityBlob blob;
            EntityBlobMetadata blobMetadata;
            try
            {
              blob = (EntityBlob)property.GetValue(Activator.CreateInstance(entity.Type));
              blobMetadata = blob.Metadata;
            }
            catch (NullReferenceException ex)
            {
              var errorBuilder = new StringBuilder();
              errorBuilder.AppendLine($"Error extracting {nameof(EntityBlob)} property {property.Name} in entity {property.EntityType.Name}");
              errorBuilder.AppendLine($"Please correctly implement {nameof(EntityBlob)} property");
              errorBuilder.AppendLine($"Exception: {ex.Message}");
              throw new InvalidOperationException(errorBuilder.ToString());
            }

            // Check whether this metadata is of the type we're currently processing. If not,
            // the type will still be processed, but not as part of this subclass.

            if (blobMetadata.EntityType != entity.Type)
              continue;

            if (!tables.TryGetValue(accessor.TableNameWithSchema, out var table))
            {
              table = new Table(accessor.SchemaName, accessor.TableName, accessor.KeyName);
              tables.Add(accessor.TableNameWithSchema, table);
            }

            if (!table.Fields.Any(p => string.Equals(p.BlobColumnName, blobMetadata.BlobColumnName, StringComparison.OrdinalIgnoreCase)))
            {
              var hashProperty = accessor.GetProperty(blobMetadata.HashMemberName);

              table.Fields.Add(new TableField(blobMetadata.BlobColumnName, hashProperty.Column.ColumnName));
            }
          }
        }
      }

      return tables
        .OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
        .Select(p => p.Value)
        .ToList();
    }

    private void MigrateTable(Table table)
    {
      if (TimeoutExpired())
        return;

      log($"Migrating table '{table.SchemaName}.{table.TableName}'");

      int? minId = FindFirstId(table);
      if (!minId.HasValue)
      {
        log("Table already has been fully migrated");
        return;
      }

      int offset = minId.Value;
      int total = 0;

      string selectQuery = BuildSelectQuery(table);
      string updateQuery = BuildUpdateQuery(table);

      while (!TimeoutExpired())
      {
        using (var session = BslDataSessionFactory.GetDataSession(userId))
        using (var transaction = session.CreateTransaction())
        {
          var records = GetRecords(table, selectQuery, offset, session);
          total += records.Count;

          if (records.Count == 0)
          {
            log($"Completed migrating {total} records");
            break;
          }

          // Put the offset after the highest id in the record list.
          offset = records.Max(p => p.Id) + 1;

          UploadRecords(records);

          SaveRecords(table, updateQuery, records, session);

          transaction.Commit();
        }
      }
    }

    private string BuildSelectQuery(Table table)
    {
      var sql = new SQLTextBuilder()
        .Text($"SELECT TOP {batchSize} ").Name(table.KeyName);

      foreach (var field in table.Fields)
      {
        sql.Text(", ").Name(field.BlobColumnName);
      }

      sql
        .Text(" ")
        .Text("FROM ").Name(table.SchemaName).Text(".").Name(table.TableName).Text(" ")
        .Text("WHERE (").Name(table.KeyName).Text(" >= ").ParameterName("@Offset").Text(") AND (");

      for (var i = 0; i < table.Fields.Count; i++)
      {
        if (i > 0)
          sql.Text(" OR ");
        sql.Name(table.Fields[i].BlobColumnName).Text(" IS NOT NULL");
      }

      sql
        .Text(") ")
        .Text("ORDER BY ").Name(table.KeyName);

      return sql.ToString();
    }

    private string BuildUpdateQuery(Table table)
    {
      var sql = new SQLTextBuilder()
        .Text("UPDATE ").Name(table.SchemaName).Text(".").Name(table.TableName).Text(" ")
        .Text("SET ");

      for (int i = 0; i < table.Fields.Count; i++)
      {
        if (i > 0)
          sql.Text(", ");

        var field = table.Fields[i];

        sql
          .Name(field.BlobColumnName).Text(" = NULL, ")
          .Name(field.HashColumnName).Text(" = ").ParameterName("@Hash" + i);
      }

      sql
        .Text(" ")
        .Text("WHERE ").Name(table.KeyName).Text(" = ").ParameterName("@Id");

      return sql.ToString();
    }

    private List<Record> GetRecords(Table table, string selectQuery, int offset, IDataSession session)
    {
      return session
        .Query(selectQuery)
        .Set("@Offset", offset)
        .UseExtendedTimeout()
        .Execute()
        .List(p =>
        {
          var record = new Record(p.Get<int>(0));

          for (int i = 0; i < table.Fields.Count; i++)
          {
            record.Values.Add(p.Get<byte[]>(i + 1));
          }

          return record;
        });
    }

    private void UploadRecords(List<Record> records)
    {
      // A version of this was tested with Task.WaitAll and an async upload record,
      // that calls the Azure Blob Storage API's async. This caused significant
      // slowdown. Parallel should be configured to a value between 2 and 4 for
      // optimal performance. In that case, async won't gain us much.

      var options = new ParallelOptions { MaxDegreeOfParallelism = parallel };

      System.Threading.Tasks.Parallel.ForEach(records, options, UploadRecord);
    }

    private void UploadRecord(Record record)
    {
      foreach (var value in record.Values)
      {
        string hash = null;

        if (value != null)
        {
          using (var stream = new MemoryStream(value))
          {
            hash = service.SaveBlob(stream);
          }
        }

        record.Hashes.Add(hash);
      }
    }

    private void SaveRecords(Table table, string updateQuery, List<Record> records, IDataSession session)
    {
      foreach (var record in records)
      {
        var query = session
          .Query(updateQuery)
          .Set("@Id", record.Id)
          .UseExtendedTimeout();

        for (int i = 0; i < table.Fields.Count; i++)
        {
          query.Set("@Hash" + i, record.Hashes[i]);
        }

        query.ExecuteNonQuery();
      }
    }

    private int? FindFirstId(Table table)
    {
      int? minId;

      using (var session = BslDataSessionFactory.GetDataSession(userId))
      using (var transaction = session.CreateTransaction())
      {
        var sql = new SQLTextBuilder()
          .Text("SELECT MIN(").Name(table.KeyName).Text(") ")
          .Text("FROM ").Name(table.SchemaName).Text(".").Name(table.TableName).Text(" ")
          .Text("WHERE ");

        for (var i = 0; i < table.Fields.Count; i++)
        {
          if (i > 0)
            sql.Text(" OR ");
          sql.Name(table.Fields[i].BlobColumnName).Text(" IS NOT NULL");
        }

        minId = session.Query(sql.ToString())
          .UseExtendedTimeout()
          .Execute()
          .SingleScalar<int?>();

        transaction.Commit();
      }

      return minId;
    }

    private bool TimeoutExpired()
    {
      if (cancellationToken.IsCancellationRequested)
      {
        log("Cancelling migration because the job has been canceled or the timeout has expired");
        return true;
      }
      return false;
    }

    public class Table
    {
      public string SchemaName { get; }

      public string TableName { get; }

      public string KeyName { get; }

      public List<TableField> Fields { get; } = new List<TableField>();

      public Table(string schemaName, string tableName, string keyName)
      {
        SchemaName = schemaName;
        TableName = tableName;
        KeyName = keyName;
      }
    }

    public class TableField
    {
      public string BlobColumnName { get; }

      public string HashColumnName { get; }

      public TableField(string blobColumnName, string hashColumnName)
      {
        BlobColumnName = blobColumnName;
        HashColumnName = hashColumnName;
      }
    }

    private class Record
    {
      public int Id { get; }

      public List<byte[]> Values { get; } = new List<byte[]>();

      public List<string> Hashes { get; } = new List<string>();

      public Record(int id)
      {
        Id = id;
      }
    }
  }
}
