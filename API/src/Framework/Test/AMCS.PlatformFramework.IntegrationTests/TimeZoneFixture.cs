namespace AMCS.PlatformFramework.IntegrationTests
{
  using AMCS.Data.Server;
  using AMCS.PlatformFramework.Entity;
  using GeoTimeZone;
  using NUnit.Framework;

  [TestFixture]
  public class TimeZoneFixture : TestBase
  {
    private const double AmcsLatitude = 52.651133613143195;
    private const double AmcsLongitude = -8.576334963599079;
    [Test]
    public void PopulateMissingTimeZones()
    {
      WithSession(session =>
      {
        var timeZoneResult = TimeZoneLookup.GetTimeZone(AmcsLatitude, AmcsLongitude);
        foreach (var location in session.GetAll<LocationEntity>(AdminUserId, false))
        {
          if (location.TimeZoneId == null)
          {
            location.TimeZoneId = timeZoneResult.Result;
            var id = session.Save(AdminUserId, location);
            Assert.IsNotNull(id);
          }
        }
      });
    }
  }
}
