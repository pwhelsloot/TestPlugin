namespace AMCS.Data.Util.Calculate
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DateTime;
  using Entity;

  public class IntervalCalculator
  {
    private class IntervalData : IIntervalEntity
    {
      public int? GetId() { return null; }
      public int? NoOfDays { get; set; }
      public int? NoOfMonths { get; set; }
      public bool IsDaily { get; set; }
      public bool IsMonthly { get; set; }
      public bool IsWeekly { get; set; }
      public string Description { get; set; }
    }

    private IIntervalEntity Interval;

    public IntervalCalculator(int? noOfDays, int? noOfMonths, bool isDaily, bool isMonthly,  bool isWeekly)
    {
      this.Interval = new IntervalData
      {
        IsDaily = isDaily,
        IsMonthly = isMonthly,
        IsWeekly = isWeekly,
        NoOfDays = noOfDays,
        NoOfMonths = noOfMonths
      };
    }

    public IntervalCalculator(IIntervalEntity interval)
    {
      this.Interval = interval;
    }

    /// <summary>
    /// Calculates the next valid date for an interval from today.
    /// </summary>
    /// <param name="lastValidDate">The last valid date for the interval.</param>
    /// <param name="dayOfWeek">The day of week.</param>
    /// <param name="occurrence">The occurrence.</param>
    /// <param name="isLastDay">if set to <c>true</c> [is last day].</param>
    /// <param name="canNextDueBeSameAsToday">if set to <c>true</c> [can next due be same as today].</param>
    /// <returns></returns>
    public System.DateTime CalculateNextValidDateFromToday(System.DateTime lastValidDate, DayOfWeekEnum? dayOfWeek, DayOccurrenceInMonthEnum? occurrence, bool isLastDay, bool canNextDueBeSameAsToday = false)
    {
      return this.CalculateNextValidDateFromDate(DateTimeUnspecified.Today, lastValidDate, dayOfWeek, occurrence, isLastDay, canNextDueBeSameAsToday);
    }

    /// <summary>
    /// Calculates the next valid date for an interval from target date.
    /// </summary>
    /// <param name="targetDate">The date to find next valid interval from.</param>
    /// <param name="lastValidDate">The last valid date.</param>
    /// <param name="dayOfWeek">The day of week.</param>
    /// <param name="occurrence">The occurrence.</param>
    /// <param name="isLastDay">if set to <c>true</c> [is last day].</param>
    /// <param name="canNextValidBeSameAsTarget">if set to <c>true</c> [can next valid be same as target].</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Calculation not possible: Interval set-up incorrect.</exception>
    public System.DateTime CalculateNextValidDateFromDate(System.DateTime targetDate, System.DateTime lastValidDate, DayOfWeekEnum? dayOfWeek, DayOccurrenceInMonthEnum? occurrence, bool isLastDay, bool canNextValidBeSameAsTarget = false)
    {
      System.DateTime nextValidDate = lastValidDate;
      // Code uses less than clause to keep in calculate loop. If our next valid date = target date it'll fall out of the loop
      // if we don't want the valid date to ever be same as target date then increment target date.
      if (!canNextValidBeSameAsTarget)
      {
        targetDate = targetDate.AddDays(1);
      }

      if (this.Interval.IsDaily)
      {
        return nextValidDate.AddDays(1);
      }

      if (dayOfWeek.HasValue)
      {
        // Keep executing calls till we have a valid date beyond target date
        while (nextValidDate < targetDate)
        {
          nextValidDate = this.GetClosestDateToSatisfyConditions(nextValidDate, dayOfWeek.Value, occurrence);
          nextValidDate = this.GetNextPickupDate(nextValidDate, occurrence);
        }
        return nextValidDate;
      }
      // If no day of week it is assumed it is a monthly with no occurrence
      else
      {
        if (this.Interval.IsWeekly || occurrence.HasValue)
          throw new Exception("Calculation not possible: Interval set-up incorrect.");

        while (nextValidDate < targetDate)
        {
          nextValidDate = nextValidDate.AddMonths(this.Interval.NoOfMonths.Value);
          if (isLastDay)
          {
            nextValidDate = nextValidDate.LastDayOfMonth();
          }
        }
        return nextValidDate;
      }
    }

    /// <summary>
    /// Returns the next Pickup date based on a previous date a pickup occurred.
    /// </summary>
    /// <param name="previousDate">Previous Date of Pickup</param>
    /// <returns>Next Date of Pickup</returns>
    public System.DateTime GetNextPickupDate(System.DateTime previousPickupDate, DayOccurrenceInMonthEnum? dayOccurence)
    {
      DayOfWeekEnum dayOfWeek = previousPickupDate.ElemosDayOfWeek();

      if (this.Interval.IsMonthly)
      {
        // Find next DayOccurrenceOfMonth in next month (ie. if previousPickupDate was 2nd Tuesday of March, find 2nd Tuesday of April)
        DayOccurrenceInMonthEnum expectedDayOccurrenceInMonth = DayOccurrenceInMonthEnum.First;

        if (dayOccurence != null)
        {
          expectedDayOccurrenceInMonth = dayOccurence.Value;
        }
        else
        {
          expectedDayOccurrenceInMonth = (DayOccurrenceInMonthEnum)previousPickupDate.GetDayOfDateOccurrentInMonth();
          if ((int)expectedDayOccurrenceInMonth == 5 || (int)expectedDayOccurrenceInMonth == 4)
            expectedDayOccurrenceInMonth = DayOccurrenceInMonthEnum.Last;
        }

        System.DateTime nextDate = new System.DateTime(previousPickupDate.Year, previousPickupDate.Month, 1);
        nextDate = nextDate.AddMonths(this.Interval.NoOfMonths.Value);
        nextDate = DayOccurrenceInMonthExtensions.GetDateOfDayOccurence(nextDate, previousPickupDate.DayOfWeek, expectedDayOccurrenceInMonth);

        return nextDate;
      }
      else
      {
        return previousPickupDate.AddDays(this.Interval.NoOfDays.GetValueOrDefault(0));
      }
    }

    /// <summary>
    /// Gets the previous pickup date.
    /// </summary>
    /// <param name="currentPickupDate">The current pickup date.</param>
    /// <param name="dayOccurence">The day occurence.</param>
    /// <returns></returns>
    public System.DateTime GetPreviousPickupDate(System.DateTime currentPickupDate, DayOccurrenceInMonthEnum? dayOccurence)
    {
      DayOfWeekEnum dayOfWeek = currentPickupDate.ElemosDayOfWeek();

      if (this.Interval.IsMonthly)
      {
        // Find next DayOccurrenceOfMonth in next month (ie. if previousPickupDate was 2nd Tuesday of March, find 2nd Tuesday of April)
        DayOccurrenceInMonthEnum expectedDayOccurrenceInMonth = DayOccurrenceInMonthEnum.First;

        if (dayOccurence != null)
        {
          expectedDayOccurrenceInMonth = dayOccurence.Value;
        }
        else
        {
          expectedDayOccurrenceInMonth = (DayOccurrenceInMonthEnum)currentPickupDate.GetDayOfDateOccurrentInMonth();
          if ((int)expectedDayOccurrenceInMonth == 5 || (int)expectedDayOccurrenceInMonth == 4)
            expectedDayOccurrenceInMonth = DayOccurrenceInMonthEnum.Last;
        }

        System.DateTime previousDate = new System.DateTime(currentPickupDate.Year, currentPickupDate.Month, 1);
        previousDate = previousDate.AddMonths(-this.Interval.NoOfMonths.Value);
        previousDate = DayOccurrenceInMonthExtensions.GetDateOfDayOccurence(previousDate, currentPickupDate.DayOfWeek, expectedDayOccurrenceInMonth);

        return previousDate;
      }
      else
      {
        return currentPickupDate.AddDays(-this.Interval.NoOfDays.GetValueOrDefault(0));
      }
    }

    /// <summary>
    /// Returns true if this Interval started on startDate will fall on targetDate.
    /// </summary>
    /// <param name="startDate">Date to start Interval from</param>
    /// <param name="targetDate">Date to confirm if this  Interval is valid for falling on this date</param>
    /// <param name="includeTimeComponentOfDates">Tells function if it should ignore the Time components of provided dates</param>
    /// <returns>Date of including permitted Margin</returns>
    public bool IsValidIntervalForDates(System.DateTime startDate, System.DateTime targetDate, DayOccurrenceInMonthEnum? dayOccurence, bool includeTimeComponentOfDates = false)
    {
      if (!includeTimeComponentOfDates)
      {
        // Ensure time component excluded from provided dates
        startDate = startDate.Date;
        targetDate = targetDate.Date;
      }

      while (startDate <= targetDate)
      {
        if (startDate == targetDate)
        {
          return true;
        }
        startDate = GetNextPickupDate(startDate, dayOccurence);
      }

      return false;
    }

    /// <summary>
    /// Gets the closest date to satisfy conditions.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="dayOfWeek">The day of week.</param>
    /// <param name="occurrence">The occurrence.</param>
    /// <returns></returns>
    public System.DateTime GetClosestDateToSatisfyConditions(System.DateTime date, DayOfWeekEnum dayOfWeek, DayOccurrenceInMonthEnum? occurrence)
    {
      var closestDate = date;
      int forwardsDelta = 0;
      int backwardsDelta = 0;

      for (int i = 0; i < 2; i++)
      {
        var currentDate = date;
        // first loop = forwards, 2nd loop = backwards
        int factor = 1;
        if (i == 1)
          factor = -1;

        bool foundDate = false;
        while (!foundDate)
        {
          var correctDayOfWeek = currentDate.ElemosDayOfWeek() == dayOfWeek;
          var correctOccurrence = true;
          if (this.Interval.IsMonthly)
            correctOccurrence = DayOccurrenceInMonthExtensions.IsDateDayExpectedOccurrenceInMonth(currentDate, occurrence.Value);

          foundDate = correctDayOfWeek && correctOccurrence;
          if (!foundDate)
          {
            currentDate = currentDate.AddDays(1 * factor);
            var temp = factor < 0 ? backwardsDelta++ : forwardsDelta++;
          }
        }
      }

      if (forwardsDelta >= backwardsDelta)
      {
        closestDate = date.AddDays(backwardsDelta * -1);
      }
      else
      {
        closestDate = date.AddDays(forwardsDelta);
      }

      return closestDate;
    }

    /// <summary>
    /// Returns the next Pickup deadline date (ie. Next Pickup Date + Margin) from provided startDate.
    /// </summary>
    /// <param name="startDate">Date to calculate from</param>
    /// <returns>Date of Pickup including this Pickup Interval's Margin</returns>
    public System.DateTime GetPickupCutOffDate(System.DateTime date, DayOccurrenceInMonthEnum? dayOccurence, int? margin)
    {
      return GetNextPickupDate(date, dayOccurence).AddDays(margin.GetValueOrDefault(0));
    }
  }
}
