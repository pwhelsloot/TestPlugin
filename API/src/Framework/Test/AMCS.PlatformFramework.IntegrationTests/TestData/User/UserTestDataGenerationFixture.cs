namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using System.Linq;
  using AMCS.Data.Server;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using AMCS.PlatformFramework.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class UserTestDataGenerationFixture : TestBase
  {
    [Test]
    public void BuildUserRecordsCreatesEntitiesInDatabase()
    {
      int numberToGenerate = 15;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(numberToGenerate), AdminUserId);
        var result = TestDataGenerationHelper.Import(builder.Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(numberToGenerate, result.ImportedRecords.Count);
        var ids = result.ImportedRecords.Select(ds => ds.GetId()).ToList();
        var userEntities = session.GetAll<UserEntity>(AdminUserId, false).Where(user => ids.Contains(user.Id.Value)).ToList();
        Assert.AreEqual(numberToGenerate, userEntities.Count);

        // Delete created test records
        userEntities.ForEach(user => session.Delete(AdminUserId, user, false));
      });
    }

    [Test]
    public void BuildUserRecordsInValidateModeDoesNotCreateEntitiesInDatabase()
    {
      int numberToGenerate = 15;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(numberToGenerate), AdminUserId);
        var result = TestDataGenerationHelper.Import(builder.SetMode(DataSetImportMode.Validate).Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(0, result.ImportedRecords.Count);
      });
    }

    [Test]
    public void BuildUserRecordsUsingClassInsteadOfResolvingServiceCreatesEntitiesInDatabase()
    {
      int numberToGenerate = 12;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        new UserTestDataGenerator().Generate(builder, configuration, new UserTestDataOptions(numberToGenerate), AdminUserId, session);
        var result = TestDataGenerationHelper.Import(builder.Build(), AdminUserId);
        Assert.AreEqual(0, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(numberToGenerate, result.ImportedRecords.Count);
        var ids = result.ImportedRecords.Select(ds => ds.GetId()).ToList();
        var userEntities = session.GetAll<UserEntity>(AdminUserId, false).Where(user => ids.Contains(user.Id.Value)).ToList();
        Assert.AreEqual(numberToGenerate, userEntities.Count);

        // Delete created test records
        userEntities.ForEach(user => session.Delete(AdminUserId, user, false));
      });
    }

    [Test]
    public void BuildUserRecordsWithEmptyUserNameDoesNotCreateEntitiesInDatabase()
    {
      int numberToGenerate = 15;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(numberToGenerate), AdminUserId);
        var import = builder.Build();

        // Empty all UserNames
        foreach (IDataSetRecord user in import.TableSet.Tables[0].Records)
        {
          ((UserRecord)user).UserName = null;
        }

        var result = TestDataGenerationHelper.Import(import, AdminUserId);
        Assert.AreEqual(numberToGenerate, result.Result.Messages.Count);
        Assert.AreEqual(numberToGenerate, result.FailedRecords.Count);
        Assert.AreEqual(0, result.ImportedRecords.Count);
      });
    }

    [Test]
    public void BuildUserRecordsWithSomeEmptyUserNameCreatesEntitiesInDatabaseForValidOnly()
    {
      int validNumberOfRecords = 10;
      int invalidNumberOfRecords = 7;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(validNumberOfRecords + invalidNumberOfRecords), AdminUserId);

        // Empty UserNames to create invalid records
        var import = builder.Build();
        foreach (IDataSetRecord user in import.TableSet.Tables[0].Records.Take(invalidNumberOfRecords))
        {
          ((UserRecord)user).UserName = null;
        }

        var result = TestDataGenerationHelper.Import(import, AdminUserId);
        Assert.IsTrue(result.Result.Messages.HasErrors);
        Assert.AreEqual(invalidNumberOfRecords, result.Result.Messages.Count);
        Assert.AreEqual(invalidNumberOfRecords, result.FailedRecords.Count);
        Assert.AreEqual(validNumberOfRecords, result.ImportedRecords.Count);

        var ids = result.ImportedRecords.Select(ds => ds.GetId()).ToList();
        var userEntities = session.GetAll<UserEntity>(AdminUserId, false).Where(user => ids.Contains(user.Id.Value)).ToList();
        Assert.AreEqual(validNumberOfRecords, userEntities.Count);

        // Delete created test records
        userEntities.ForEach(user => session.Delete(AdminUserId, user, false));
      });
    }

    [Test]
    public void BuildUserRecordsWithSomeEmptyUserNameInValidateModeValidatesCorrectly()
    {
      int validNumberOfRecords = 10;
      int invalidNumberOfRecords = 7;
      var configuration = new PlatformFrameworkTestDataConfiguration();
      WithSession(session =>
      {
        var builder = new TestDataBuilder("sampleSeed");
        TestDataGenerationHelper.GenerateUserTestData(builder, configuration, new UserTestDataOptions(validNumberOfRecords + invalidNumberOfRecords), AdminUserId);

        // Empty UserNames to create invalid records
        var import = builder.SetMode(DataSetImportMode.Validate).Build();
        foreach (IDataSetRecord user in import.TableSet.Tables[0].Records.Take(invalidNumberOfRecords))
        {
          ((UserRecord)user).UserName = null;
        }

        var result = TestDataGenerationHelper.Import(import, AdminUserId);
        Assert.IsTrue(result.Result.Messages.HasErrors);
        Assert.AreEqual(invalidNumberOfRecords, result.Result.Messages.Count);
        Assert.AreEqual(0, result.FailedRecords.Count);
        Assert.AreEqual(0, result.ImportedRecords.Count);
      });
    }
  }
}
