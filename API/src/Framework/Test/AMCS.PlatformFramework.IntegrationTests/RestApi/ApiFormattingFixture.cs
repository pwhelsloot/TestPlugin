namespace AMCS.PlatformFramework.IntegrationTests.RestApi
{
  using System;
  using System.Linq;
  using AMCS.Data;
  using AMCS.Data.Entity;
  using AMCS.Data.Server;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Server.Api;
  using AMCS.PlatformFramework.Server.User;
  using Newtonsoft.Json.Linq;
  using NodaTime;
  using NUnit.Framework;

  [TestFixture]
  public class ApiFormattingFixture : TestBase
  {
    private static readonly LocalDateTime Now = new LocalDateTime(2020, 4, 2, 10, 21);
    private static readonly ZonedDateTime NowUtc = Now.InZoneStrictly(DateTimeZone.Utc);

    [Test]
    public void SimpleGet()
    {
      var job = CreateJob();

      string actual = ApiFixtureUtils.GetControllerInternalOutput<JobEntity>(
        AdminUserId,
        p => p.Get(job.JobId.Value, null, null, null, null)
      );

      string expected = $@"
{{
  ""resource"": {{
    ""JobId"": {job.JobId},
     ""Date"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/Dublin""
    }},
    ""Created"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/London""
    }},
    ""Modified"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/London""
    }},
    ""Executed"": null,
    ""CustomerSiteId"": {job.CustomerSiteId},
    ""LocationTimeZoneId"": ""Europe/Dublin"",
    ""DateRequired"": null,
    ""StartTimeRequired"": null,
    ""EndTimeRequired"": null,
    ""StartTimeRequiredZoned"": null,
    ""EndTimeRequiredZoned"": null,
    ""GUID"": ""{job.GUID}"",
    ""LastChangeReasonId"": null
  }}
}}
";
      AssertJsonEqual(expected, actual);
    }

    [Test]
    public void GetWithLocalDateAndTime()
    {
      var job = CreateJob(p =>
      {
        p.DateRequired = Now.Date;
        p.StartTimeRequired = Now.TimeOfDay;
        p.EndTimeRequired = Now.TimeOfDay + Period.FromHours(1);
      });

      string actual = ApiFixtureUtils.GetControllerInternalOutput<JobEntity>(
        AdminUserId,
        p => p.Get(job.JobId.Value, null, null, null, null)
      );

      Console.WriteLine(PrettifyJson(actual));

      string expected = $@"
{{
  ""resource"": {{
    ""JobId"": {job.JobId},
    ""Date"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/Dublin""
    }},
    ""Created"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/London""
    }},
    ""Modified"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/London""
    }},
    ""Executed"": null,
    ""CustomerSiteId"": {job.CustomerSiteId},
    ""LocationTimeZoneId"": ""Europe/Dublin"",
    ""DateRequired"": ""2020-04-02"",
    ""StartTimeRequired"": ""10:21:00"",
    ""EndTimeRequired"": ""11:21:00"",
    ""StartTimeRequiredZoned"": {{
      ""DateTime"": ""2020-04-02T10:21:00+01:00"",
			""TimeZone"": ""Europe/Dublin""
    }},
    ""EndTimeRequiredZoned"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/Dublin""
    }},
    ""GUID"": ""{job.GUID}"",
    ""LastChangeReasonId"": null
  }}
}}
";
      AssertJsonEqual(expected, actual);
    }

    [Test]
    public void SaveWithLocalDateAndTime()
    {
      var job = CreateJob();

      string post = $@"
{{
 ""Date"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/Dublin""
    }},
    ""Created"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/London""
    }},
    ""Modified"": {{
      ""DateTime"": ""2020-04-02T11:21:00+01:00"",
			""TimeZone"": ""Europe/London""
    }},
  ""Executed"": null,
  ""CustomerSiteId"": {job.CustomerSiteId},
  ""LocationTimeZoneId"": ""Europe/Dublin"",
  ""DateRequired"": ""2020-04-02"",
  ""StartTimeRequired"": ""10:21:00"",
  ""EndTimeRequired"": ""11:21:00""
}}
";

      string response = ApiFixtureUtils.GetControllerInternalOutput<JobEntity>(
        AdminUserId,
        p => p.Create(),
        post
      );

      int jobId = (int)JObject.Parse(response)["resource"];

      var actualJob = WithSession(ctx => ctx.GetById<JobEntity>(AdminUserId, jobId));

      Assert.AreEqual(Now.Date, actualJob.DateRequired);
      Assert.AreEqual(Now.TimeOfDay, actualJob.StartTimeRequired);
      Assert.AreEqual(Now.TimeOfDay + Period.FromHours(1), actualJob.EndTimeRequired);
    }

    [Test]
    public void EnsureNoBom()
    {
      var user = WithSession(ctx => DataServices.Resolve<IUserService>().GetAllById(AdminUserId, 0, false, ctx).First());

      var output = ApiFixtureUtils.GetControllerEntityMessageBinaryOutput<ApiGetUserNameService, UserEntity, ApiGetUserNameService.Request, ApiGetUserNameService.Response>(
        AdminUserId,
        user.Id32
      );

      Assert.IsFalse(HasBom(output));
    }

    private static bool HasBom(byte[] output)
    {
      return
        output.Length >= 3 &&
        output[0] == 0xef &&
        output[1] == 0xbb &&
        output[2] == 0xbf;
    }

    private JobEntity CreateJob(Action<JobEntity> jobAction = null)
    {
      return WithSession(ctx =>
      {
        var location = Create<LocationEntity>(ctx, p =>
        {
          p.Address = GetRandomLabel("Address");
          p.TimeZoneId = "Europe/Dublin";
        });

        var customerSite = Create<CustomerSiteEntity>(ctx, p =>
        {
          p.Name = GetRandomLabel("Name");
          p.LocationId = location.Id32;
        });

        return Create<JobEntity>(ctx, p =>
        {
          p.CustomerSiteId = customerSite.Id32;
          p.Date = NowUtc;
          p.Created = NowUtc;
          p.Modified = NowUtc;

          jobAction?.Invoke(p);
        });
      });
    }

    private T Create<T>(IDataSession ctx, Action<T> action = null)
      where T : EntityObject, new()
    {
      var entity = new T();

      action?.Invoke(entity);

      int id = ctx.Save(AdminUserId, entity).Value;

      return ctx.GetById<T>(AdminUserId, id);
    }
  }
}
