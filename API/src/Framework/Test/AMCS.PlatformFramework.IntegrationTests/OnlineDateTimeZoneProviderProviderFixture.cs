using System;
using System.Data.SqlClient;
using AMCS.Data.Configuration;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Server;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests
{
  [TestFixture]
  public class OnlineDateTimeZoneProviderProviderFixture : TestBase
  {
    [Test]
    public void CreateOnlineDateTimeZoneProviderFromEmbeddedTimeZoneDatabase()
    {
      var connectionString = ConnectionStringEncryption.DecryptFromConfiguration("PlatformFrameworkConnectionString");
      DeleteCachedDbTimeZoneDatabase(connectionString);

      var onlineDateTimeZoneProviderProvider = new OnlineDateTimeZoneProviderProvider(connectionString, string.Empty, TimeSpan.FromDays(1));

      //// verify timezone database was saved to database
      var cachedTimezoneDatabase = GetCachedTimeZoneDatabase(connectionString);
      Assert.IsNotNull(cachedTimezoneDatabase, "Failed to save timezoneDatabase to db");
      Assert.AreEqual("(embedded)", cachedTimezoneDatabase.Name);
      Assert.IsFalse(cachedTimezoneDatabase.Online);
      
      //// verify source and provider were publicized and assigned
      Assert.IsNotNull(onlineDateTimeZoneProviderProvider.DateTimeZoneSource, "Failed to create TzdbDateTimeZoneSource");
      Assert.IsNotNull(onlineDateTimeZoneProviderProvider.DateTimeZoneProvider, "Failed to create DateTimeZoneCache");
    }

    [Test]
    public void CreateOnlineDateTimeZoneProviderFromCachedTimeZoneDatabase()
    {
      var connectionString = ConnectionStringEncryption.DecryptFromConfiguration("PlatformFrameworkConnectionString");
      DeleteCachedDbTimeZoneDatabase(connectionString);

      var onlineDateTimeZoneProviderProvider1 = new OnlineDateTimeZoneProviderProvider(connectionString, string.Empty, TimeSpan.FromDays(1));

      //// verify timezone database was saved to database
      var cachedTimezoneDatabase1 = GetCachedTimeZoneDatabase(connectionString);
      Assert.IsNotNull(cachedTimezoneDatabase1, "Failed to save timezoneDatabase to db");

      var onlineDateTimeZoneProviderProvider2 = new OnlineDateTimeZoneProviderProvider(connectionString, string.Empty, TimeSpan.FromDays(1));

      //// verify timezone database was loaded from database
      var cachedTimezoneDatabase2 = GetCachedTimeZoneDatabase(connectionString);
      Assert.IsNotNull(cachedTimezoneDatabase2, "Failed to save timezoneDatabase to db");
      Assert.AreEqual(cachedTimezoneDatabase1.Guid, cachedTimezoneDatabase2.Guid);

      //// verify source and provider were publicized and assigned
      Assert.AreEqual(onlineDateTimeZoneProviderProvider1.DateTimeZoneSource.VersionId, onlineDateTimeZoneProviderProvider2.DateTimeZoneSource.VersionId);
      Assert.AreEqual(onlineDateTimeZoneProviderProvider1.DateTimeZoneProvider.VersionId, onlineDateTimeZoneProviderProvider2.DateTimeZoneProvider.VersionId);
    }

    private static void DeleteCachedDbTimeZoneDatabase(IConnectionString connectionString)
    {
      using (var connection = new SqlConnection(connectionString.GetConnectionString()))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "delete from [dbo].[DBTimeZoneDatabase]";
          command.ExecuteNonQuery();
        }
      }
    }

    private static DbTimeZoneDatabase GetCachedTimeZoneDatabase(IConnectionString connectionString)
    {
      DbTimeZoneDatabase dbTimeZoneDatabase = null;

      using (var connection = new SqlConnection(connectionString.GetConnectionString()))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "select [Name], [Data], [Online], [GUID] from [dbo].[DBTimeZoneDatabase]";
          using (var reader = command.ExecuteReader())
          {
            if (reader.Read())
              dbTimeZoneDatabase = new DbTimeZoneDatabase { Name = (string)reader["Name"], Data = (byte[])reader["Data"], Online = (bool)reader["Online"], Guid = (Guid)reader["GUID"] };
            Assert.IsFalse(reader.Read());

            ////while (reader.Read())
            ////{
            ////  dbTimeZoneDatabase = new DbTimeZoneDatabase { Name = (string)reader["Name"], Data = (byte[])reader["Data"], Online = (bool)reader["Online"], Guid = (Guid)reader["GUID"] };
            ////}
          }
        }
      }

      return dbTimeZoneDatabase;
    }

    private class DbTimeZoneDatabase
    {
      public string Name { get; set; }
      public byte[] Data { get; set; }
      public bool Online { get; set; }
      public Guid Guid { get; set; }
    }
  }
}
