namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Server;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Server.DataSets.SystemConfiguration;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using AMCS.PlatformFramework.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class TestDataGenerationFixture : TestBase
  {
    [Test]
    public void BuildMultipleRecordTypesUsingClassInsteadOfResolveCreatesEntitiesInDatabase()
    {
      int numberOfUsersToGenerate = 20;
      int numberOfSystemConfigurationsToGenerate = 10;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(numberOfUsersToGenerate), AdminUserId);
        TestDataGenerationHelper.GenerateSystemConfigurationTestData(builder, configuration, new SystemConfigurationTestDataOptions(numberOfSystemConfigurationsToGenerate), AdminUserId);
        var result = TestDataGenerationHelper.Import(builder.Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(numberOfUsersToGenerate + numberOfSystemConfigurationsToGenerate, result.ImportedRecords.Count);
        var userIds = result.ImportedRecords.Where(record => record is UserRecord).Select(ds => ds.GetId()).ToList();
        var userEntities = session.GetAll<UserEntity>(AdminUserId, false).Where(user => userIds.Contains(user.Id.Value)).ToList();
        Assert.AreEqual(numberOfUsersToGenerate, userEntities.Count);
        var configIds = result.ImportedRecords.Where(record => record is SystemConfigurationRecord).Select(ds => ds.GetId()).ToList();
        var configEntities = session.GetAll<SystemConfigurationEntity>(AdminUserId, false).Where(config => configIds.Contains(config.Id.Value)).ToList();
        Assert.AreEqual(numberOfSystemConfigurationsToGenerate, configEntities.Count);

        // Delete created test records
        userEntities.ForEach(user => session.Delete(AdminUserId, user, false));
        configEntities.ForEach(config => session.Delete(AdminUserId, config, false));
      });
    }
  }
}
