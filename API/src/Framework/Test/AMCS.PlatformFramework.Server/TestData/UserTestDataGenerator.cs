namespace AMCS.PlatformFramework.Server.TestData
{
  using System.Linq;
  using AMCS.Data;
  using AMCS.Data.Server;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.DataSets.User;
  using FizzWare.NBuilder;

  public class UserTestDataGenerator : IPlatformFrameworkTestDataGenerator<UserTestDataOptions>
  {
    public void Generate(TestDataBuilder builder, PlatformFrameworkTestDataConfiguration configuration, UserTestDataOptions options, ISessionToken userId, IDataSession dataSession)
    {
      var items = FizzWare.NBuilder.Builder<UserRecord>.CreateListOfSize(options.Number)
        .All()
        .Do((user, index) => user.UserId = (index + 1) * -1)
        .With(user => user.UserName = builder.NextRandomString())
        .With(user => user.Email = $"{builder.NextRandom()}@{options.EmailDomain}")
        .Build()
        .ToList<IDataSetRecord>();

      builder.AddRecords(items);
    }
  }
}
