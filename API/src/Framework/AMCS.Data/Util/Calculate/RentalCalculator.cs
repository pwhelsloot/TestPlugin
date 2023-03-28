namespace AMCS.Data.Util.Calculate
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static class RentalCalculator
  {
    public enum MonthDifferenceMeasurementType
    {
      WholeMonthsOnly,
      WholeMonthsPlusLastMonthFraction,
      WholeMonthsPlusFirstMonthFraction,
    }

    //to account for leap year.
    private static double DAYS_PER_YEAR = 365;
    
    //public static void GetRentalStartDateDecoded(DateTime rentalStartDate,  out int startYear,  out int startMonth, out int startDay)
    //{
    //  startDay = rentalStartDate.Day;
    //  startMonth = rentalStartDate.Month;
    //  startYear = rentalStartDate.Year;

    //  if (startDay == 1)
    //  {
    //    startDay = 0;
    //  }

    //  if (startDay == DateTime.DaysInMonth(startYear, startMonth))
    //  {
    //    startDay = 0;
    //    startMonth = startMonth + 1;

    //    if (startMonth == 13)
    //    {
    //      startMonth = 1;
    //      startYear = startYear + 1;
    //    }
    //  }
    //}

    //public static void GetRentalEndDateDecoded(DateTime rentalEndDate, out int endYear, out int endMonth, out int endDay)
    //{
    //  endDay = rentalEndDate.Day;
    //  endMonth = rentalEndDate.Month;
    //  endYear = rentalEndDate.Year;
    //}

    //public static decimal GetRentalPriceFactorFractionStart(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, int days, int weeks)
    //{
    //  if (days != 0 || weeks != 0 || startDay == 0)
    //  {
    //    return 0;
    //  }

    //  if (startMonth == endMonth && endDay != DateTime.DaysInMonth(startYear, startMonth))
    //  {
    //    return 0;
    //  }

    //  return (Convert.ToDecimal(DateTime.DaysInMonth(startYear, startMonth)) - Convert.ToDecimal(startDay)) / Convert.ToDecimal(DateTime.DaysInMonth(startYear, startMonth));
    //}

    //public static decimal GetRentalPriceFactorFractionEnd(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, int days, int weeks)
    //{
    //  if (endDay == DateTime.DaysInMonth(endYear, endMonth))
    //  {
    //    return 0;
    //  }

    //  if (days != 0 || weeks != 0)
    //  {
    //    return 0;
    //  }

    //  int startDayToCalc = 0;
    //  if (endMonth == startMonth)
    //  {
    //    startDayToCalc = startDay;
    //  }

    //  return Convert.ToDecimal(endDay-startDayToCalc)/Convert.ToDecimal(DateTime.DaysInMonth(endYear, endMonth));
    //}

    //public static decimal GetRentalDailyWeeklyPriceFactor(DateTime rentalFrom, DateTime rentalTo, bool rentalToIsPickup, int days, int weeks, out DateTime roundRentalToDate)
    //{
    //  int daysbetween = (int)(rentalTo.Date - rentalFrom.Date).TotalDays;
    //  daysbetween = daysbetween + 1;

    //  int totalDays = days + (weeks * 7);
      
    //  if (rentalToIsPickup)
    //  {
    //    roundRentalToDate = rentalTo;
    //    return Convert.ToDecimal(daysbetween) / Convert.ToDecimal(totalDays);
    //  }
    //  else
    //  {
    //    roundRentalToDate = rentalTo;
    //    decimal factor = Math.Truncate(Convert.ToDecimal(daysbetween) / Convert.ToDecimal(totalDays));
    //    roundRentalToDate = rentalFrom.AddDays(Convert.ToDouble(factor) * Convert.ToDouble(totalDays));
    //    return  factor;        
    //  }
    //}

    //public static bool IsMonthlyRentalDue(DateTime rentalTo, DateTime? rentalOrderStartDate, DateTime? rentalLastActionDate)
    //{
    //  DateTime rentalBaseDate = GetMaxDateFromList(new List<DateTime?> { rentalOrderStartDate, rentalLastActionDate });

    //  return rentalTo >= rentalBaseDate;
    //}

    //public static DateTime GetMonthlyRentalEndDate(int months, DateTime startDate)
    //{
    //  return startDate.AddMonths(months).AddDays(-1);
    //}

    //public static decimal GetMonthlyRentalFactor(int months, DateTime startDate, DateTime? endDate, bool arrears)
    //{
    //  if (endDate == null)
    //  {
    //    return 1;
    //  }
    //  else
    //  {
    //    DateTime periodEnd = GetMonthlyRentalEndDate(months, startDate);

    //    if (arrears)
    //    {
    //      DateTime endOfMonth = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month));
    //      DateTime startOfMonth = new DateTime(endDate.Value.Year, endDate.Value.Month, 1);

    //      // check whether rent term end date falls on end of a month
    //      if (endDate.Value < endOfMonth)
    //      {
    //        TimeSpan tsTotal = endOfMonth - startOfMonth;
    //        TimeSpan tsActual = Convert.ToDateTime(endDate.Value) - startOfMonth;

    //        decimal factor = (((decimal)tsActual.Days / (decimal)tsTotal.Days) / months) + (((decimal)(endDate.Value.Month - startDate.Month)) / months); 

    //        return Math.Round(factor, 2);
    //      }
    //      else
    //        return ((endDate.Value.Month - startDate.Month + 1) / months);
    //    }
    //    else if (periodEnd <= endDate)
    //    {
    //      return 1;
    //    }
    //    else
    //    {
    //      TimeSpan tsTotal = periodEnd - startDate;
    //      TimeSpan tsActual = Convert.ToDateTime(endDate) - startDate;

    //      decimal factor = (decimal)tsActual.Days / (decimal)tsTotal.Days;

    //      return Math.Round(factor, 2);
    //    }
    //  }
    //}

    //public static DateTime GetMaxDateFromList(List<DateTime?> dates)
    //{
    //  DateTime maxDate = DateTime.MinValue;

    //  foreach (DateTime? dateToCheck in dates)
    //  {
    //    if (dateToCheck != null)
    //    {
    //      if (dateToCheck > maxDate)
    //      {
    //        maxDate = (DateTime)dateToCheck;
    //      }
    //    }
    //  }

    //  return maxDate;
    //}

    public static DateTime? GetMinDateFromList(List<DateTime?> dates)
    {
      DateTime? minDate = null;

      foreach (DateTime? dateToCheck in dates)
      {
        if (dateToCheck != null)
        {
          if (minDate == null)
          {
            minDate = dateToCheck;
          }
          else if (dateToCheck < minDate)
          {
            minDate = (DateTime)dateToCheck;
          }
        }
      }

      return minDate;
    }

    #region Pro-Rata

    /*
    //public const int daysInMonth = 30;
    public static decimal GetMonthlyProRataMonthlyRentalFactor(int months, int quantity, DateTime startDate, DateTime endDate, decimal price)
    {
      decimal priceFactor = Convert.ToDecimal(GetMonthlyTermPriceFactor(startDate, endDate, months, MonthDifferenceMeasurementType.WholeMonthsPlusFirstMonthFraction));
      return Math.Round((priceFactor * price) * (decimal)quantity, 2);

      //TimeSpan ts =  endDate - startDate;

      //int daysToBill = ts.Days;
      //int daysInPeriod = daysInMonth * months;

      //decimal factor = (decimal)daysToBill / (decimal)daysInPeriod;

      //return Math.Round((factor * price)*(decimal)quantity, 2);
    }
    */

    public static decimal GetProRataCharge(int? days, int? months, decimal quantity, DateTime startDate, DateTime endDate, decimal price, int? yearlyDistributionMonths = null)
    {
      if (days.GetValueOrDefault(0) > 0 && months.GetValueOrDefault(0) > 0)
        throw new Exception("System Error: Illegal attempt to calculate a pro-rata charge on a term of " + days.Value + " days and " + months.Value + " months.  Can only calculate for months or days, not a combination of the two.");
      if (days.GetValueOrDefault(0) <= 0 && months.GetValueOrDefault(0) <= 0)
        throw new Exception("System Error: Illegal attempt to calculate a pro-rata charge on a term of " + days.GetValueOrDefault(0) + " days and " + months.GetValueOrDefault(0) + " months.  Either days or months must have a positive value.");

      decimal priceFactor = 0;
      if (months.GetValueOrDefault(0) > 0)
        priceFactor = Convert.ToDecimal(GetMonthlyTermPriceFactor(startDate, endDate, months.Value, MonthDifferenceMeasurementType.WholeMonthsPlusFirstMonthFraction));
      else //daily
        priceFactor = Convert.ToDecimal(GetDailyTermPriceFactor(startDate, endDate, days.Value, yearlyDistributionMonths));
      
      return Math.Round((priceFactor * price) * quantity, 2);
    }

    #endregion Pro-Rata

    #region Price Factor

    /// <summary>
    /// Gets the daily term price factor.
    /// If "yearlyDistributionMonths" > 0 then the price factor will be calculated in such a way that there will be (12 / yearlyDistributionMonths) equal
    /// price factors in a year - leading to (12 / yearlyDistributionMonths) equal charges in a year.
    /// </summary>
    /// <param name="fromDate">From date.</param>
    /// <param name="toDate">To date.</param>
    /// <param name="dailyTerm">The daily term.</param>
    /// <param name="yearlyDistributionMonths">The yearly distribution months.</param>
    /// <returns></returns>
    public static double GetDailyTermPriceFactor(DateTime fromDate, DateTime toDate, int dailyTerm, int? yearlyDistributionMonths = null)
    {
      if (yearlyDistributionMonths.GetValueOrDefault(0) > 0)
      {
        double monthDifference = GetMonthDifference(fromDate, toDate, MonthDifferenceMeasurementType.WholeMonthsPlusLastMonthFraction);
        //if (monthDifference == Math.Floor(monthDifference))
        //{
          //365.25 days a year to account for leap years
          if (monthDifference < yearlyDistributionMonths)
          {
            double monthlyDistributionPeriods = monthDifference / (double)yearlyDistributionMonths;
            double priceFactor = monthlyDistributionPeriods * ((DAYS_PER_YEAR / (double)dailyTerm) / (12 / yearlyDistributionMonths.Value));
            return priceFactor;
          }
          else if (monthDifference % yearlyDistributionMonths.Value == 0)
          {
            int monthlyDistributionPeriods = Convert.ToInt32(monthDifference / yearlyDistributionMonths.Value);
            
            double priceFactor = monthlyDistributionPeriods * ((DAYS_PER_YEAR / (double)dailyTerm) / (12 / yearlyDistributionMonths.Value));
            return priceFactor;
          }
        //}
      }
      double rentalDays = toDate.AddDays(1).Subtract(fromDate).TotalDays;
      return rentalDays / dailyTerm;
    }

    /// <summary>
    /// Gets the monthly term price factor.
    /// </summary>
    /// <param name="fromDate">From date.</param>
    /// <param name="toDate">To date.</param>
    /// <param name="monthlyTerm">The monthly term.</param>
    /// <param name="monthDifferenceMeasurementType">Type of the month difference measurement.</param>
    /// <returns></returns>
    public static double GetMonthlyTermPriceFactor(DateTime fromDate, DateTime toDate, int monthlyTerm, MonthDifferenceMeasurementType monthDifferenceMeasurementType)
    {
      double monthlyDifference = GetMonthDifference(fromDate, toDate, monthDifferenceMeasurementType);
      return monthlyDifference / monthlyTerm;
    }

    /// <summary>
    /// Gets the month difference.
    /// </summary>
    /// <param name="fromDate">From date.</param>
    /// <param name="toDate">To date.</param>
    /// <param name="monthDifferenceMeasurementType">Type of the month difference measurement.</param>
    /// <returns></returns>
    public static double GetMonthDifference(DateTime fromDate, DateTime toDate, MonthDifferenceMeasurementType monthDifferenceMeasurementType)
    {
      //don't want times involved
      DateTime workingFromDate = fromDate.Date;
      DateTime workingToDate = toDate.Date.AddDays(1);

      if (workingFromDate > workingToDate)
        throw new Exception("System Error: fromDate is after toDate");

      if (workingFromDate == workingToDate)
        return 0;

      //does the period cover complete months, e.g. 1st Jan to 28th Feb or 10th Jan to 9th of March.
      int monthDifference = ((workingToDate.Year - workingFromDate.Year) * 12) + workingToDate.Month - workingFromDate.Month;
      if (workingFromDate.Day == workingToDate.Day)
        return monthDifference;
      if (workingFromDate.Day > workingToDate.Day)
        monthDifference--;

      double fraction = 0;
      if (monthDifferenceMeasurementType == MonthDifferenceMeasurementType.WholeMonthsPlusFirstMonthFraction)
        fraction = GetFirstMonthlyFraction(workingFromDate, workingToDate);
      else if (monthDifferenceMeasurementType == MonthDifferenceMeasurementType.WholeMonthsPlusLastMonthFraction)
        fraction = GetLastMonthlyFraction(workingFromDate, workingToDate);
      //else MonthDifferenceMeasurementType.WholeMonthsOnly - so don't want a fraction
      
      return monthDifference + fraction;
    }

    /// <summary>
    /// Gets the first monthly fraction.
    /// </summary>
    /// <param name="fromDate">From date.</param>
    /// <param name="toDate">To date.</param>
    /// <returns></returns>
    private static double GetFirstMonthlyFraction(DateTime fromDate, DateTime toDate)
    {
      //this is the date that the last rental period would start on
      DateTime previousPeriodStartDate = toDate.AddMonths(-1);
      //the days in this single rental period
      int daysInPeriod = (int)(toDate.Subtract(previousPeriodStartDate).Ticks / TimeSpan.TicksPerDay);
      //the number of days in the period which are not fully covered by a period.
      //if all days in the period being looked at are covered then this number will be negative
      int daysNotCoveredByPeriod = (int)(fromDate.Subtract(previousPeriodStartDate).Ticks / TimeSpan.TicksPerDay);

      if (daysNotCoveredByPeriod > 0)
        return (daysInPeriod - daysNotCoveredByPeriod) / (double)daysInPeriod;
      //we're not in the first period yet, try again.
      return GetFirstMonthlyFraction(fromDate, previousPeriodStartDate);
    }

    /// <summary>
    /// Gets the last monthly fraction.
    /// </summary>
    /// <param name="fromDate">From date.</param>
    /// <param name="toDate">To date.</param>
    /// <returns></returns>
    private static double GetLastMonthlyFraction(DateTime fromDate, DateTime toDate, DateTime? originalFromDate = null)
    {
      if (originalFromDate == null)
        originalFromDate = fromDate;

      //this is the date that a single rental period should end on
      DateTime periodEndDate = fromDate.AddMonths(1);

      if (DateTime.DaysInMonth(periodEndDate.Year, periodEndDate.Month) >= originalFromDate.Value.Day)
      {
        int driftDays = originalFromDate.Value.Day - periodEndDate.Day;
        if (driftDays > 0)
          periodEndDate = periodEndDate.AddDays(driftDays);
      }

      //the days in this single rental period
      int daysInPeriod = (int)(periodEndDate.Subtract(fromDate).Ticks / TimeSpan.TicksPerDay);
      //the actual days we want to calculate the rental over, this may span multiple periods
      int actualDays = (int)(toDate.Subtract(fromDate).Ticks / TimeSpan.TicksPerDay);


      //it is possible that there is a large range between the start and end dates, spanning multiple
      //rental periods. We are only interested in the last, incomplete, period so repeat this process
      //but the start dates will now become that of the next periods start date.
      if (actualDays <= daysInPeriod)
        return actualDays / (double)daysInPeriod;

      return GetLastMonthlyFraction(periodEndDate, toDate, originalFromDate);
    }

    #endregion Price Factor
  }
}
