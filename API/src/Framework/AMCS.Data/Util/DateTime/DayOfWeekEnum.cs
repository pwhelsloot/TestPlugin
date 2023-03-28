namespace AMCS.Data.Util.DateTime
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.Serialization;
  using Extension;

  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/elemos")]
  public enum DayOfWeekEnum
  {
    [EnumMember]
    [StringValue("Monday")]
    dwMonday = 1,
    [EnumMember]
    [StringValue("Tuesday")]
    dwTuesday = 2,
    [EnumMember]
    [StringValue("Wednesday")]
    dwWednesday = 3,
    [EnumMember]
    [StringValue("Thursday")]
    dwThursday = 4,
    [EnumMember]
    [StringValue("Friday")]
    dwFriday = 5,
    [EnumMember]
    [StringValue("Saturday")]
    dwSaturday = 6,
    [EnumMember]
    [StringValue("Sunday")]
    dwSunday = 7
  }
}
