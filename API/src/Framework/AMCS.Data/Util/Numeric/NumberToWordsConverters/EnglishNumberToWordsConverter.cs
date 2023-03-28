namespace AMCS.Data.Util.Numeric.NumberToWordsConverters
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class EnglishNumberToWordsConverter : INumberToWordsConverter
  {

    public string Convert(int number)
    {
      return Convert(System.Convert.ToDecimal(number));
    }

    public string Convert(short number)
    {
      return Convert(System.Convert.ToDecimal(number));
    }

    public string Convert(long number)
    {
      return Convert(System.Convert.ToDecimal(number));
    }

    public string Convert(double number)
    {
      return Convert(System.Convert.ToDecimal(number));
    }

    /// <summary>
    /// Converts number to a string in English
    /// </summary>
    public string Convert(decimal number)
    {
      throw new NotImplementedException("");
    }
  }
}
