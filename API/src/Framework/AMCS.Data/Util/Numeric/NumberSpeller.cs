namespace AMCS.Data.Util.Numeric
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Text;

  public class NumberSpeller
  {
    #region Properties

    private CultureInfo _culture;
    private INumberToWordsConverter _convertor;

    #endregion Properties

    #region Constructors

    public NumberSpeller(CultureInfo culture)
    {
      _culture = culture;
      DetermineConverter();
    }

    public NumberSpeller() : this(CultureInfo.CurrentCulture) { }

    #endregion Constructors

    #region Public Methods

    public string Spell(int number)
    {
      return _convertor.Convert(number);
    }

    public string Spell(short number)
    {
      return _convertor.Convert(number);
    }

    public string Spell(long number)
    {
      return _convertor.Convert(number);
    }

    public string Spell(double number)
    {
      return _convertor.Convert(number);
    }

    public string Spell(decimal number)
    {
      return _convertor.Convert(number);
    }

    #endregion Public Methods

    #region Private Methods

    private void DetermineConverter()
    {
      string cultureCode = _culture.Name;
      switch (cultureCode)
      {
        case "en-US":
        case "en-GB":
        case "en-IE":
          _convertor = new NumberToWordsConverters.EnglishNumberToWordsConverter();
          break;
        case "fr-FR":
          _convertor = new NumberToWordsConverters.FranceFrenchNumberToWordsConverter();
          break;
        default:
          throw new NotImplementedException(cultureCode + " culture not yet supported.");
      }
    }

    #endregion Private Methods
  }
}
