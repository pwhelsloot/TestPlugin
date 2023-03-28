namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using System.Linq;
  using AMCS.Data.Server;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class TestDataOptionsFixture : TestBase
  {
    [Test]
    [TestCase(8, null)]
    [TestCase(17, "amcsgroup.com")]
    public void BuildUserRecordsWithValidOptionsCreatesEntitiesInDatabaseUsingOptionsValues(int number, string emailDomain)
    {
      var configuration = new PlatformFrameworkTestDataConfiguration();
      var userTestDataOptions = new UserTestDataOptions(number, emailDomain);
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, userTestDataOptions, AdminUserId);
        var result = TestDataGenerationHelper.Import(builder.Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(userTestDataOptions.Number, result.ImportedRecords.Count);
        var ids = result.ImportedRecords.Select(record => record.GetId()).ToList();
        var userEntities = session.GetAll<UserEntity>(AdminUserId, false).Where(user => ids.Contains(user.Id.Value)).ToList();
        Assert.AreEqual(userTestDataOptions.Number, userEntities.Count);
        Assert.AreEqual(userTestDataOptions.Number, userEntities.Count(user => user.EmailAddress.EndsWith(userTestDataOptions.EmailDomain)));

        // Delete created test records
        userEntities.ForEach(user => session.Delete(AdminUserId, user, false));
      });
    }

    [Test]
    public void BuildUserRecordsWithDefaultValidOptionsCreatesEntitiesInDatabaseUsingDefaultOptionsValues()
    {
      var userTestDataOptions = new UserTestDataOptions(15);
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, userTestDataOptions, AdminUserId);
        var result = TestDataGenerationHelper.Import(builder.Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(userTestDataOptions.Number, result.ImportedRecords.Count);
        var ids = result.ImportedRecords.Select(record => record.GetId()).ToList();
        var userEntities = session.GetAll<UserEntity>(AdminUserId, false).Where(user => ids.Contains(user.Id.Value)).ToList();
        Assert.AreEqual(userTestDataOptions.Number, userEntities.Count);
        Assert.AreEqual(userTestDataOptions.Number, userEntities.Count(user => user.EmailAddress.EndsWith(userTestDataOptions.EmailDomain)));

        // Delete created test records
        userEntities.ForEach(user => session.Delete(AdminUserId, user, false));
      });
    }
  }
}
