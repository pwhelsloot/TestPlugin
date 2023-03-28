namespace AMCS.Data.Util.DateTime
{
  using System.Runtime.Serialization;
  using System;
  using Extension;

  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/elemos")]
  public enum DayOccurrenceInMonthEnum
  {
    [EnumMember]
    [StringValue("1st")]
    First = 1,

    [EnumMember]
    [StringValue("2nd")]
    Second = 2,

    [EnumMember]
    [StringValue("3rd")]
    Third = 3,

    [EnumMember]
    [StringValue("4th")]
    Fourth = 4,

    [EnumMember]
    [StringValue("Last")]
    Last = 5,
  }
}