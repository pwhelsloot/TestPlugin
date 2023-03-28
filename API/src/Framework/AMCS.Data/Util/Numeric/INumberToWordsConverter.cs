namespace AMCS.Data.Util.Numeric
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public interface INumberToWordsConverter
  {
    string Convert(int number);

    string Convert(short number);

    string Convert(long number);

    string Convert(double number);

    string Convert(decimal number);
  }
}
