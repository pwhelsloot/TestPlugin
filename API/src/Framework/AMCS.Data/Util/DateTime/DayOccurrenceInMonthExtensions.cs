namespace AMCS.Data.Util.DateTime
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static class DayOccurrenceInMonthExtensions
  {
    /// <summary>
    /// Returns true if provided DateTime is occurence of the DayOccurrenceInMonth's occurrence
    /// </summary>
    /// <param name="date">The date to check</param>
    /// <param name="expectedDayOccurrenceInMonth">The occurrence of the month date is expected to be</param>
    public static bool IsDateDayExpectedOccurrenceInMonth(System.DateTime date, DayOccurrenceInMonthEnum expectedDayOccurrenceInMonth)
    {
      if (expectedDayOccurrenceInMonth == DayOccurrenceInMonthEnum.Last)
      {
        System.DateTime tempDate = date;
        int occurenceNumber = tempDate.GetDayOfDateOccurrentInMonth();
        int totalOccurences = occurenceNumber;
        while (tempDate.Month == date.Month)
        {
          tempDate = tempDate.AddDays(7);
          if (tempDate.Month == date.Month)
            totalOccurences++;
        }

        return occurenceNumber == totalOccurences;
      }
      else
      {
        return date.GetDayOfDateOccurrentInMonth() == (int)expectedDayOccurrenceInMonth;
      }
    }

    /// <summary>
    /// RDM - DO NOT USE. This method used to cause an infinite loop (Bug 101051) and so has been made to just return false as it shouldn't ever be used. This would 
    /// be removed but we don't want a breaking change for it.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>
    ///   <c>true</c> if [is date day expected occurrence in month] [the specified date]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDateDayExpectedOccurrenceInMonth(System.DateTime? date)
    {
      return false;
    }

    public static bool IsDateDayExpectedOccurrenceInMonth(System.DateTime date, DayOccurrenceInMonthEnum? dayOccurrenceInMonth)
    {
      if (!dayOccurrenceInMonth.HasValue) { return false; }
      return IsDateDayExpectedOccurrenceInMonth(date, dayOccurrenceInMonth.Value);
    }

    /// <summary>
    /// Finds the date in the month that satisfies the day occurence
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="dayOfWeek">The day of week.</param>
    /// <param name="dayOccurrence">The day occurrence.</param>
    /// <returns></returns>
    public static System.DateTime GetDateOfDayOccurence(System.DateTime date, DayOfWeek dayOfWeek, DayOccurrenceInMonthEnum dayOccurrence)
    {
      var potentialDate = date.Next(dayOfWeek);

      bool found = false;
      while (!found)
      {
        found = IsDateDayExpectedOccurrenceInMonth(potentialDate, dayOccurrence);

        if (!found)
          potentialDate = potentialDate.AddDays(7);
      }

      return potentialDate;
    }

  }
}
