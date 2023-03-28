namespace AMCS.Data.Util.DateTime
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;

  public static class DateTimeExtensions
  {
    public static DateTime LastDayOfMonth(this DateTime date)
    {
      DateTime firstDayOfTheMonth = new DateTime(date.Year, date.Month, 1);
      return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
    }

    public static DateTime FirstDayOfNextMonth(this DateTime date)
    {
      DateTime firstDayOfTheMonth = new DateTime(date.Year, date.Month, 1);
      return firstDayOfTheMonth.AddMonths(1);
    }

    [Obsolete("Use PickupIntervalEntity.GetPickupCutOffDate(Datetime date) instead.")] 
    public static DateTime PickupCutOffByDays(this DateTime date, int NoOfDays, int Margin)
    {
      DateTime pickupDate = date.AddDays(-1 * NoOfDays).AddDays(Margin);
      return pickupDate;
    }

    public static DateTime GetLatestDate(this DateTime date, DateTime dateToCheck)
    {
      if (dateToCheck > date)
      {
        return dateToCheck;
      }
      else
      {
        return date;
      }
    }

    public static DayOfWeekEnum ElemosDayOfWeek(this DateTime date)
    {
      DayOfWeekEnum day;
      switch (date.DayOfWeek)
      {
        case DayOfWeek.Friday:
          day = DayOfWeekEnum.dwFriday;
          break;
        case DayOfWeek.Monday:
          day = DayOfWeekEnum.dwMonday;
          break;
        case DayOfWeek.Saturday:
          day = DayOfWeekEnum.dwSaturday;
          break;
        case DayOfWeek.Sunday:
          day = DayOfWeekEnum.dwSunday;
          break;
        case DayOfWeek.Thursday:
          day = DayOfWeekEnum.dwThursday;
          break;
        case DayOfWeek.Tuesday:
          day = DayOfWeekEnum.dwTuesday;
          break;
        case DayOfWeek.Wednesday:
          day = DayOfWeekEnum.dwWednesday;
          break;
        default:
          throw new Exception("ELEMOS Day Of Week Out of Bounds.");
      }

      return day;
    }

    static GregorianCalendar _GregorianCalendar = new GregorianCalendar();
    public static int GetWeekOfYear(this DateTime time, DayOfWeek firstDayOfWeek=DayOfWeek.Sunday)
    {
      return _GregorianCalendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, firstDayOfWeek);
    }

    public static int GetWeekOfMonth(this DateTime time, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
    {
      DateTime first = new DateTime(time.Year, time.Month, 1);
      return time.GetWeekOfYear(firstDayOfWeek) - first.GetWeekOfYear(firstDayOfWeek) + 1;
    }

    /// <summary>
    /// Returns the occurrent of the date's week day within its month. (eg. a date of 11 March 2013 (a Monday) would return 2. It is the 2nd Monday of March 2013)
    /// </summary>
    /// <param name="date">The date to return the occurrence of</param>
    /// <returns>The number of times the date's day has previously occurred in its current month</returns>
    public static int GetDayOfDateOccurrentInMonth(this DateTime date)
    {
      return Convert.ToInt32(Math.Ceiling(date.Day / 7.00));
    }

    public static bool IsLastDayOfDateOccurrentInMonth(this DateTime date)
    {
        DayOfWeek dayOfWeek = date.DayOfWeek;
        DateTime LastDateOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        DayOfWeek lastDayOfMonth = LastDateOfMonth.DayOfWeek;

        int diff = dayOfWeek - lastDayOfMonth;

        if (diff > 0) diff -= 7;

        System.Diagnostics.Debug.Assert(diff <= 0);

        DateTime calcDateofLastDayOccurence = LastDateOfMonth.AddDays(diff);

        return calcDateofLastDayOccurence == date;
    }

    ///<summary>Gets the first week day following a date.</summary>
    ///<param name="date">The date.</param>
    ///<param name="dayOfWeek">The day of week to return.</param>
    ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
    public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
    {
        return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
    }

    /// <summary>
    /// Returns a string for the date that is in an unambiguous format
    /// </summary>
    /// <param name="dateTime">A datetimedat</param>
    /// <returns>A ISO 8601 string for the date</returns>
    public static string ToUnambiguousString(this DateTime dateTime)
    {
      return dateTime.ToString("s", CultureInfo.InvariantCulture);
    } 
  }
}
