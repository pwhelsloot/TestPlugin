using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data.Support;
using log4net;
using Microsoft.Azure.Services.AppAuthentication;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.Data.Configuration.TimeZones
{
  public class OnlineDateTimeZoneProviderProvider : IDateTimeZoneProviderProvider
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(OnlineDateTimeZoneProviderProvider));
    public const string DefaultLatestUri = "https://nodatime.org/tzdb/latest.txt";
    public static readonly TimeSpan DefaultUpdateInterval = TimeSpan.FromDays(1);

    private readonly IConnectionString connectionString;
    private readonly string latestUri;
    private volatile IDateTimeZoneProvider dateTimeZoneProvider;
    private volatile TzdbDateTimeZoneSource dateTimeZoneSource;
    private string lastTzdb;
    private Timer timer;
    private bool disposed;

    public IDateTimeZoneProvider DateTimeZoneProvider => dateTimeZoneProvider;

    public TzdbDateTimeZoneSource DateTimeZoneSource => dateTimeZoneSource;

    public event EventHandler DateTimeZoneProviderChanged;

    public OnlineDateTimeZoneProviderProvider(IConnectionString connectionString)
      : this(connectionString, DefaultLatestUri, DefaultUpdateInterval)
    {
    }

    public OnlineDateTimeZoneProviderProvider(IConnectionString connectionString, string latestUri, TimeSpan updateInterval)
    {
      this.connectionString = connectionString;
      this.latestUri = latestUri;

      GetProvider();

      timer = new Timer(ReloadProviderCallback, null, TimeSpan.FromDays(1) - DateTime.UtcNow.TimeOfDay, updateInterval);
    }

    /// <summary>
    /// This may fail if there's an issue loading the time zone database.
    /// The expected behavior is that we go through startup again, and retry the whole process.
    /// </summary>
    private void GetProvider()
    {
      var cachedDbTimeZoneDatabase = GetCachedTimeZoneDatabase();
      if (cachedDbTimeZoneDatabase != null)
      {
        SetProvider(cachedDbTimeZoneDatabase);
      }
      else
      {
        ReloadProvider();
      }
    }

    private void ReloadProvider()
    {
      string timezoneDatabase;
      byte[] timezoneDatabaseData;

      try
      {
        using (var client = new WebClient())
        {
          timezoneDatabase = client.DownloadString(latestUri).Trim();
          if (timezoneDatabase == lastTzdb)
            return;

          timezoneDatabaseData = client.DownloadData(timezoneDatabase);
        }

        WriteCachedTimeZoneDatabase(new DbTimeZoneDatabase { Name = timezoneDatabase, Data = timezoneDatabaseData, Online = true });
      }
      catch
      {
        timezoneDatabase = "(embedded)";
        if (timezoneDatabase == lastTzdb)
          return;

        using (var stream = typeof(TzdbDateTimeZoneSource).Assembly.GetManifestResourceStream("NodaTime.TimeZones.Tzdb.nzd"))
        using (var memoryStream = new MemoryStream())
        {
          stream.CopyTo(memoryStream);
          timezoneDatabaseData = memoryStream.ToArray();
        }

        WriteCachedTimeZoneDatabase(new DbTimeZoneDatabase { Name = timezoneDatabase, Data = timezoneDatabaseData, Online = false });
      }
    }

    private void SetProvider(DbTimeZoneDatabase dbTimeZoneDatabase)
    {
      DateTimeZoneCache provider;
      TzdbDateTimeZoneSource source;

      using (var stream = new MemoryStream(dbTimeZoneDatabase.Data))
      {
        source = TzdbDateTimeZoneSource.FromStream(stream);
        provider = new DateTimeZoneCache(source);
      }

      // Publicize.

      lastTzdb = dbTimeZoneDatabase.Name;
      dateTimeZoneProvider = provider;
      dateTimeZoneSource = source;

      OnDateTimeZoneProviderChanged();
    }

    private void ReloadProviderCallback(object state)
    {
      try
      {
        Logger.Info($"Reloading TZDB at {DateTime.UtcNow:o}");
        ReloadProvider();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to refresh the TZDB", ex);
      }
    }

    protected virtual void OnDateTimeZoneProviderChanged()
    {
      DateTimeZoneProviderChanged?.Invoke(this, EventArgs.Empty);
    }

    private void WriteCachedTimeZoneDatabase(DbTimeZoneDatabase dbTimeZoneDatabase)
    {
      //// Check if a dbTimeZoneDatabase exists
      var existingDbTimeZoneDatabase = GetCachedTimeZoneDatabase();
      if (existingDbTimeZoneDatabase != null)
      {
        //// if the existing is one we got online, it should only be overriden with an online version
        if (existingDbTimeZoneDatabase.Online)
        {
          if (dbTimeZoneDatabase.Online)
          {
            //// update if we have got one online
            UpdateCachedTimeZoneDatabase(dbTimeZoneDatabase, true);
            SetProvider(dbTimeZoneDatabase);
          }
          else
          {
            //// use existing online one if the one we got was from embedded
            SetProvider(existingDbTimeZoneDatabase);
          }
        }
        else
        {
          //// if existing is from embedded update with whatever we have just got
          UpdateCachedTimeZoneDatabase(dbTimeZoneDatabase, true);
          SetProvider(dbTimeZoneDatabase);
        }
      }
      else
      {
        //// if no existing write to database
        UpdateCachedTimeZoneDatabase(dbTimeZoneDatabase, false);
        SetProvider(dbTimeZoneDatabase);
      }
    }

    private DbTimeZoneDatabase GetCachedTimeZoneDatabase()
    {
      var query = "select [Name], [Data], [Online] from [dbo].[DBTimeZoneDatabase]";
      DbTimeZoneDatabase dbTimeZoneDatabase = null;

      using (var connection = ConnectionStringUtils.OpenSqlConnection(connectionString))
      using (var command = connection.CreateCommand())
      {
        command.CommandText = query;
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            dbTimeZoneDatabase = new DbTimeZoneDatabase { Name = (string)reader["Name"], Data = (byte[])reader["Data"], Online = (bool)reader["Online"] };
          }
        }
      }

      return dbTimeZoneDatabase;
    }

    private void UpdateCachedTimeZoneDatabase(DbTimeZoneDatabase dbTimeZoneDatabase, bool exists)
    {
      var query = exists
        ? "update [dbo].[DBTimeZoneDatabase] set [Name] = @Name, [Data] = @Data, [Online] = @Online"
        : "insert into [dbo].[DBTimeZoneDatabase]([Name], [Data], [Online]) VALUES(@Name, @Data, @Online)";

      using (var connection = ConnectionStringUtils.OpenSqlConnection(connectionString))
      using (var command = connection.CreateCommand())
      {
        command.CommandText = query;
        command.Parameters.AddWithValue("@Name", dbTimeZoneDatabase.Name);
        command.Parameters.AddWithValue("@Data", dbTimeZoneDatabase.Data);
        command.Parameters.AddWithValue("@Online", dbTimeZoneDatabase.Online);
        command.ExecuteNonQuery();
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (timer != null)
        {
          timer.Dispose();
          timer = null;
        }

        disposed = true;
      }
    }

    private class DbTimeZoneDatabase
    {
      public string Name { get; set; }
      public byte[] Data { get; set; }
      public bool Online { get; set; }
    }
  }
}
