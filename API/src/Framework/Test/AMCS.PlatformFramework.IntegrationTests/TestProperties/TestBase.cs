using System;
using System.IO;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using AMCS.Data.Support.Security;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests
{
  using Server.Configuration;

  public class TestBase
  {
    private readonly Random random = SharedRandom.GetRandom();

    protected TestBase()
    {
      TestServiceSetup.Setup();

      AdminUserId = GetAdminUserId();
    }

    public ISessionToken AdminUserId { get; }

    private static ISessionToken GetAdminUserId()
    {
      var userService = DataServices.Resolve<IUserService>();

      return userService.CreateSessionToken((AuthenticationUser)userService.Authenticate(
        new UserNameCredentials("admin", DataServices.Resolve<IPlatformFrameworkConfiguration>().TenantId)));
    }

    protected static void WithSession(Action<IDataSession> action)
    {
      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        action(session);

        transaction.Commit();
      }
    }

    protected static T WithSession<T>(Func<IDataSession, T> action)
    {
      T result;

      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        result = action(session);

        transaction.Commit();
      }

      return result;
    }

    protected static void AssertJsonEqual(string expected, string actual)
    {
      Assert.AreEqual(PrettifyJson(expected), PrettifyJson(actual));
    }

    protected static string PrettifyJson(string json)
    {
      using (var reader = new StringReader(json))
      using (var jsonReader = new JsonTextReader(reader))
      using (var writer = new StringWriter())
      {
        using (var jsonWriter = new JsonTextWriter(writer))
        {
          jsonWriter.Formatting = Formatting.Indented;
          jsonWriter.WriteToken(jsonReader);
        }

        return writer.ToString();
      }
    }

    protected string GetRandomLabel(string prefix)
    {
      return prefix + "_" + random.Next(100000, 999999);
    }
  }
}
