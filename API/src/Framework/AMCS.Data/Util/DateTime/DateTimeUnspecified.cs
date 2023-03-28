namespace AMCS.Data.Util.DateTime
{
  using System;

  /// <summary>
  /// Utility class to provide helper methods for DateTime in unspecified format.
  /// </summary>
  public static class DateTimeUnspecified
  {
    /// <summary>
    /// Gets DateTime.Now with the kind marked as unspecified
    /// </summary>
    /// <value>DateTime.Now with kind on date time as unspecified as opposed to local</value>
    public static System.DateTime Now
    {
      get
      {
        return System.DateTime.SpecifyKind(System.DateTime.Now, DateTimeKind.Unspecified);
      }
    }

    /// <summary>
    /// Gets DateTime.Today with the kind marked as unspecified
    /// </summary>
    /// <value>DateTime.Today with kind on date time as unspecified as opposed to local</value>
    public static System.DateTime Today
    {
      get
      {
        return System.DateTime.SpecifyKind(System.DateTime.Today, DateTimeKind.Unspecified);
      }
    }
  }
}
