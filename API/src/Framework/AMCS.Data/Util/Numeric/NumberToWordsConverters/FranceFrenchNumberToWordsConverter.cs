namespace AMCS.Data.Util.Numeric.NumberToWordsConverters
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class FranceFrenchNumberToWordsConverter : INumberToWordsConverter
  {

    #region Properties

    Dictionary<decimal, string> _numberTranslations = new Dictionary<decimal,string>() {
      { 0, "zéro" },
      { 1, "un" },
      { 2, "deux" },
      { 3, "trois" },
      { 4, "quatre" },
      { 5, "cinq" },
      { 6, "six" },
      { 7, "sept" },
      { 8, "huit" },
      { 9, "neuf" },
      { 10, "dix" },
      { 11, "onze" },
      { 12, "douze" },
      { 13, "treize" },
      { 14, "quatorze" },
      { 15, "quinze" },
      { 16, "seize" },
      { 17, "dix-sept" },
      { 18, "dix-huit" },
      { 19, "dix-neuf" },
      { 20, "vingt" },
      { 30, "trente" },
      { 40, "quarante" },
      { 50, "cinquante" },
      { 60, "soixante" },
      { 70, "soixante-dix" },
      { 71, "soixante et onze" },
      { 72, "soixante-douze" },
      { 80, "quatre-vingts" },
      { 81, "quatre-vingt-un" },
      { 90, "quatre-vingt-dix" },
      { 91, "quatre-vingt-onze" }
    };

    Dictionary<string, string> _strings = new Dictionary<string, string>() {
      { "and", "et" },
      { "billion", "milliard" },
      { "billions", "milliards" },
      { "hundred", "cent" },
      { "hundreds", "cents" },
      { "million", "million" },
      { "millions", "millions" },
      { "quadrillion", "billiard" },
      { "quadrillions", "billiards" },
      { "quintillion", "trillion" },
      { "quintillions", "trillions" },
      { "trillion", "billion" },
      { "trillions", "billions" },
      { "thousand", "mille" },
      { "thousands", "mille" },
      { "point", "virgule" }
    };

    #endregion Properties

    #region Public Methods

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
    /// Converts number to a string in French
    /// </summary>
    public string Convert(decimal number)
    {
      //Written with thanks to http://www.french-linguistics.co.uk/tutorials/numbers/
      //and http://www.france-pub.com/french/french_numbers.htm#.U48s8yjyT00
      string output = "";
      decimal quintillions, quadrillions, trillions, billions, millions, thousands, ones, decimals;
      decimal brokenDownTotal = 0;
      quintillions = Math.Floor(number / 1000000000000000000);
      brokenDownTotal += quintillions * 1000000000000000000;
      quadrillions = Math.Floor((number - brokenDownTotal) / 1000000000000000);
      brokenDownTotal += quadrillions * 1000000000000000;
      trillions = Math.Floor((number - brokenDownTotal) / 1000000000000);
      brokenDownTotal += trillions * 1000000000000;
      billions = Math.Floor((number - brokenDownTotal) / 1000000000);
      brokenDownTotal += billions * 1000000000;
      millions = Math.Floor((number - brokenDownTotal) / 1000000);
      brokenDownTotal += millions * 1000000;
      thousands = Math.Floor((number - brokenDownTotal) / 1000);
      brokenDownTotal += thousands * 1000;
      ones = Math.Floor(number - brokenDownTotal);
      brokenDownTotal += ones;

      //Convert decimal portion to string, remove "0.", parse back to decimal
      decimals = decimal.Parse((number - brokenDownTotal).ToString("0.0############").Remove(0, 2)); 

      if (quintillions > 0)
      {
        if (quintillions == 1)
        {
          output += TranslateNumber(quintillions) + " " + _strings["quintillion"];
        }
        else
        {
          output += TranslateNumber(quintillions) + " " + _strings["quintillions"];
        }
      }

      if (quadrillions > 0)
      {
        if (output.Length > 0) { output += " "; }
        if (quadrillions == 1)
        {
          output += TranslateNumber(quadrillions) + " " + _strings["quadrillion"];
        }
        else
        {
          output += TranslateNumber(quadrillions) + " " + _strings["quadrillions"];
        }
      }

      if (trillions > 0)
      {
        if (output.Length > 0) { output += " "; }
        if (trillions == 1)
        {
          output += TranslateNumber(trillions) + " " + _strings["trillion"];
        }
        else
        {
          output += TranslateNumber(trillions) + " " + _strings["trillions"];
        }
      }

      if (billions > 0)
      {
        if (output.Length > 0) { output += " "; }
        if (billions == 1)
        {
          output += TranslateNumber(billions) + " " + _strings["billion"];
        }
        else
        {
          output += TranslateNumber(billions) + " " + _strings["billions"];
        }
      }

      if (millions > 0)
      {
        if (output.Length > 0) { output += " "; }
        if (millions == 1)
        {
          output += TranslateNumber(millions) + " " + _strings["million"];
        }
        else
        {
          output += TranslateNumber(millions) + " " + _strings["millions"];
        }
      }

      if (thousands > 0)
      {
        if (output.Length > 0) { output += " "; }
        if (thousands == 1)
        {
          output += _strings["thousand"];
        }
        else
        {
          output += TranslateNumber(thousands) + " " + _strings["thousands"];
        }
      }

      //Don't output zero if larger digits already translated, but do output zero if nothing larger translated yet
      if (output.Length == 0 || ones > 0)
      {
        if (output.Length > 0) { output += " "; }
        output += TranslateNumber(ones);
      }

      if (decimals > 0)
      {
        //Translate decimal portion
        output += " " + _strings["point"] + " " + Convert(decimals);
      }

      return output;

    }

    #endregion Public Methods

    #region Private Methods

    private string TranslateNumber(decimal number)
    {
      if (number > 999 || number < 0) { throw new ArgumentOutOfRangeException("This method is for translating numbers less than 1000. Negative numbers are not permitted."); }
      
      if (_numberTranslations.ContainsKey(number)) { 
        //Exact translation exists, use it
        return _numberTranslations[number]; 
      } else {

        string output = "";

        //Breakdown and translate number following general rules
        decimal brokenDownTotal = 0;
        decimal hundreds = Math.Floor(number / 100);
        brokenDownTotal += hundreds * 100;
        decimal tens = Math.Floor((number - brokenDownTotal) / 10);
        brokenDownTotal += tens * 10;
        decimal ones = number - brokenDownTotal;

        if (hundreds > 0)
        {
          if (hundreds == 1)
          {
            output += _strings["hundred"];
          }
          else
          {
            output += TranslateNumber(hundreds) + " " + _strings["hundreds"];
            if (tens > 0 || ones > 0)
            {
              //If appending any tens or ones to the amount, remove last "s"
              output = output.Remove(output.LastIndexOf("s"), 1);
            }
          }
        }

        //Translate Tens
        if (tens > 0)
        {
          if (output.Length > 0) { output += " "; }

          //In french
          //the 70s are pronounced sixty-ten, sixty-eleven etc.
          //and 90s are pronounced eighty-ten, eighty-elevent etc.
          if (tens == 7 || tens == 9)
          {
            //So shift tens down by one
            tens -= 1;
            //Increase ones by ten
            ones += 10;
          }

          if (_numberTranslations.ContainsKey(tens * 10))
          {
            //Ones to come, so need to end with a "-"
            output += _numberTranslations[tens * 10] + ((ones > 0) ? "-" : "");

            //There is a special case for "quatre-vingts" because it ends with an "s" where if there is a digit to follow the ending "s" must be removed.
            if (_numberTranslations[tens * 10].EndsWith("s"))
            {
              output = output.Remove(output.LastIndexOf("s"), 1);
            }
          }
          else
          {
            throw new NotImplementedException("Translation of number " + 10 * tens + " required, but is missing.");
          }

          if (ones == 1)
          {
            //Remove end "-" and replace with " and "
            output = output.Remove(output.LastIndexOf("-")) + " " + _strings["and"] + " ";
          }
        }

        if(ones > 0 || (tens == 0 && hundreds == 0))
        //Translate Ones
        if (_numberTranslations.ContainsKey(ones))
        {
          output += _numberTranslations[ones];
        }
        else
        {
          throw new NotImplementedException("Translation of number " + ones + " required, but is missing.");
        }

        return output;
      }
    }

    #endregion Private Methods

  }
}
