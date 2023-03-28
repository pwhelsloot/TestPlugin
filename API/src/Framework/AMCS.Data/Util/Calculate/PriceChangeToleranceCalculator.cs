namespace AMCS.Data.Util.Calculate
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class PriceChangeToleranceCalculator
  {
    public static bool IsPriceChangeWithinTolerance(decimal newPrice, decimal oldPrice, decimal tolerancePercentage, decimal toleranceAmount)
    {
      if (newPrice < 0) { newPrice = newPrice * -1; }
      if (oldPrice < 0) { oldPrice = oldPrice * -1; }
      return IsPriceChangeWithinPercentageTolerance(newPrice, oldPrice, tolerancePercentage) && IsPriceChangeWithinAbsoluteTolerance(newPrice, oldPrice, toleranceAmount);
    }

    public static bool IsPriceChangeWithinPercentageTolerance(decimal newPrice, decimal oldPrice, decimal tolerancePercentage)
    {
      return newPrice >= (oldPrice * (1 - (tolerancePercentage / 100)));
    }

    public static bool IsPriceChangeWithinAbsoluteTolerance(decimal newPrice, decimal oldPrice, decimal toleranceAmount)
    {
      return newPrice >= (oldPrice - toleranceAmount);
    }
  }
}
