namespace AMCS.Data.Util.Calculate
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class PriceRoundCalculator
  {
    public const int decimalsForInvoice = 2;

    public static decimal RoundPriceForInvoice(decimal value)
    {
      return Math.Round(value, decimalsForInvoice);
    }

    public static decimal SwedishRoundPriceForInvoice(decimal value, out decimal roundingValue)
    {
      roundingValue = value - Math.Floor(value);
      if (roundingValue == 0)
        return 0M;
      else if (roundingValue < 0.5M)
      {
        roundingValue = roundingValue * -1;
        return Math.Round(value, 0);
      }
      else
      {
        if (Math.Sign(value) == -1 && roundingValue == 0.5M)
        {
          roundingValue = roundingValue * -1;
        }
        else
        {
          roundingValue = 1 - roundingValue;
        }
        return Math.Round(value, 0, MidpointRounding.AwayFromZero);
      }
    }
  }
}
