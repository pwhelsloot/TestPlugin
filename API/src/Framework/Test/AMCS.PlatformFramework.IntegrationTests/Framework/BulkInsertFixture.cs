namespace AMCS.PlatformFramework.IntegrationTests.Framework
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data;
  using AMCS.Data.Entity.Heartbeat;
  using AMCS.Data.Server;
  using AMCS.Data.Server.Heartbeat;
  using AMCS.Data.Server.Services;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.IntegrationTests.Heartbeat;
  using NUnit.Framework;
  
  [TestFixture]
  public class BulkInsertFixture : TestBase
  {
    [Test]
    public void GivenNonDboEntityList_WhenBulkSaving_ThenRecordsPersisted()
    {
      var minQuantityForBulkSave = DataServices
        .Resolve<ISettingsService>()
        .GetInteger("MinQuantityForBulkSave", 10) + 1;

      var heartbeatService = new HeartbeatService(new BulkInsertFixtureRegistry(minQuantityForBulkSave));
      heartbeatService.SyncConnectionRegistry();

      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();

        Assert.AreEqual(minQuantityForBulkSave, heartbeatConnections.Count);
      });
    }

    [Test]
    public void GivenDboEntityList_WhenBulkSaving_ThenRecordsPersisted()
    {
      var minQuantityForBulkSave = DataServices
        .Resolve<ISettingsService>()
        .GetInteger("MinQuantityForBulkSave", 10) + 1;

      var users = new List<UserEntity>();

      for (var i = 0; i < minQuantityForBulkSave; i++)
      {
        users.Add(new UserEntity
        {
          UserName = Guid.NewGuid().ToString("N"),
          EmailAddress = Guid.NewGuid().ToString("N"),
          Password = Guid.NewGuid().ToString("N"),
        });
      }

      WithSession(session =>
      {
        session.BulkSave(AdminUserId, users);

        var dbUsers = session
          .GetAll<UserEntity>(AdminUserId, true)
          .ToList();

        foreach (var user in users)
        {
          var createdUser = dbUsers.SingleOrDefault(dbUser => dbUser.UserName == user.UserName);
          Assert.IsNotNull(createdUser);

          session.Delete<UserEntity>(AdminUserId, createdUser.Id32, false);
        }
      });
    }
  }

  public class BulkInsertFixtureRegistry : IConnectionRegistry
  {
    private readonly List<ICommServerConnection> commServerConnections = new List<ICommServerConnection>();

    public TimeSpan MaxHeartbeatLatency { get; } = TimeSpan.FromMinutes(10);

    public BulkInsertFixtureRegistry(int totalConnections)
    {
      for (var i = 0; i < totalConnections; i++)
      {
        commServerConnections.Add(new CommServerConnectionBuilder().Build());
      }
    }

    public IList<ICommServerConnection> GetCommServerConnections()
    {
      return commServerConnections;
    }
  }
}