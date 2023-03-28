namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using System;
  using System.Linq;
  using System.Reflection;
  using AMCS.Data.Server;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Server.DataSets.SystemConfiguration;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using AMCS.PlatformFramework.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class SystemConfigurationTestDataGenerationFixture : TestBase
  {
    [Test]
    public void BuildSystemConfigurationRecordsWithoutCreatingDepenciesWillThrowException()
    {
      int numberToGenerate = 20;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        Assert.Throws<TargetInvocationException>(() =>
          TestDataGenerationHelper.GenerateSystemConfigurationTestData(builder, configuration, new SystemConfigurationTestDataOptions(numberToGenerate), AdminUserId)
          );
      });
    }

    [Test]
    public void BuildSystemConfigurationRecordsCreatesEntitiesInDatabase()
    {
      int numberToGenerate = 20;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        // Create a single dedendeny user record
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(1), AdminUserId);
        TestDataGenerationHelper.GenerateSystemConfigurationTestData(builder, configuration, new SystemConfigurationTestDataOptions(numberToGenerate), AdminUserId);
        var result = TestDataGenerationHelper.Import(builder.Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        var configIds = result.ImportedRecords.Where(record => record is SystemConfigurationRecord).Select(ds => ds.GetId()).ToList();
        var userId = result.ImportedRecords.Where(record => record is UserRecord).Single().GetId();
        var configEntities = session.GetAll<SystemConfigurationEntity>(AdminUserId, false).Where(config => configIds.Contains(config.Id.Value)).ToList();
        var userEntity = session.GetById<UserEntity>(AdminUserId, userId);
        Assert.AreEqual(numberToGenerate, configEntities.Count);
        Assert.NotNull(userEntity);

        // Delete created test records
        configEntities.ForEach(config => session.Delete(AdminUserId, config, false));
        session.Delete(AdminUserId, userEntity, false);
      });
    }
  }
}
