using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;
using NodaTime;
using NodaTime.TimeZones;

namespace AMCS.PlatformFramework.Entity
{
  [EntityTable("Job", "JobId")]
  [Serializable]
  public class JobEntity : EntityObject
  {
    [EntityMember]
    public int? JobId { get; set; }

    [EntityMember(DateStorage = DateStorage.Zoned, TimeZoneMember = nameof(LocationTimeZoneId))]
    public ZonedDateTime Date { get; set; }

    [EntityMember(DateStorage = DateStorage.Neutral)]
    public ZonedDateTime Created { get; set; }

    [EntityMember(DateStorage = DateStorage.Neutral)]
    public ZonedDateTime Modified { get; set; }

    [EntityMember(DateStorage = DateStorage.UTC)]
    public ZonedDateTime? Executed { get; set; }

    [EntityMember]
    public int CustomerSiteId { get; set; }

    [EntityMember(IsDynamic = true)]
    public string LocationTimeZoneId { get; set; }

    [EntityMember(DateStorage = DateStorage.Date)]
    public LocalDate? DateRequired { get; set; }

    [EntityMember(DateStorage = DateStorage.Time)]
    public LocalTime? StartTimeRequired { get; set; }

    [EntityMember(DateStorage = DateStorage.Time)]
    public LocalTime? EndTimeRequired { get; set; }

    [EntityMember(IsDynamic = true)]
    public ZonedDateTime? StartTimeRequiredZoned => ToZonedDateTime(DateRequired, StartTimeRequired);

    [EntityMember(IsDynamic = true)]
    public ZonedDateTime? EndTimeRequiredZoned => ToZonedDateTime(DateRequired, EndTimeRequired);

    private ZonedDateTime? ToZonedDateTime(LocalDate? date, LocalTime? time)
    {
      if (!(date.HasValue && time.HasValue))
        return null;

      return (date.Value + time.Value).InZoneLeniently(TimeZoneUtils.DateTimeZoneProvider[LocationTimeZoneId]);
    }

    public override int? GetId() => JobId;
  }
}
