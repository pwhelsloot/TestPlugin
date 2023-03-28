namespace AMCS.Data.Util.Decimal
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;
  using System.Threading;

  public static class DecimalExtensions
  {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="precision">total max digits</param>
    /// <param name="scale">num digits allowed after decimal point</param>
    /// <returns></returns>
    public static bool IsValid(this decimal value, int precision, int scale)
    {
      CultureInfo cult = Thread.CurrentThread.CurrentCulture;
      int afterPointMax = scale;
      int totalMax = precision;

      var strVal = Math.Abs(value).ToString();
      int beforePoint = strVal.IndexOf(cult.NumberFormat.NumberDecimalSeparator);
      int afterPoint = beforePoint == -1 ? 0 : strVal.Length - beforePoint - 1;
      beforePoint = beforePoint == -1 ? strVal.Length : beforePoint;

      return (afterPoint <= afterPointMax) && (beforePoint + scale <= totalMax);
    }
  }
}
