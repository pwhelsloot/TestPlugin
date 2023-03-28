namespace AMCS.PlatformFramework.IntegrationTests.Translations
{
  using System.Globalization;
  using AMCS.Data;
  using AMCS.Data.Configuration.Resource;
  using Data.Configuration.Mapping.Translate;
  using Entity;
  using NUnit.Framework;

  [TestFixture]
  public class TranslationFixture : TestBase
  {
    [Test]
    public void GivenTranslations_GetUnlocalisedStringAndLocaliseWithoutParameters()
    {
      // cache the initial ui culture
      var initialUICulture = CultureInfo.CurrentUICulture;

      try
      {
        // change to french
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-fr");

        var translator = new BusinessObjectStringTranslator(this.GetType().FullName, typeof(ErrorCode));

        // get the unlocalised translation
        string unLocalisedString = translator.GetUnLocalisedString((int)ErrorCode.SomeUserError);

        // verify that it comes back in english
        Assert.AreEqual("{\"Text\":\"This is an error\",\"Arguments\":null}", unLocalisedString);

        // get the localised translation
        string localisedString = translator.LocaliseUnlocalisedString(unLocalisedString);

        // verify that it comes back in french
        Assert.AreEqual("C'est une erreur", localisedString);
      }
      finally
      {
        // reset current ui culture
        CultureInfo.CurrentUICulture = initialUICulture;
      }
    }

    [Test]
    public void GivenTranslations_GetUnlocalisedStringAndLocaliseWithParameters()
    {
      // cache the initial ui culture
      var initialUICulture = CultureInfo.CurrentUICulture;

      try
      {
        // change to french
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-fr");

        var translator = new BusinessObjectStringTranslator(this.GetType().FullName, typeof(ErrorCode));

        // get the unlocalised translation
        string unLocalisedString = translator.GetUnLocalisedString(
          (int)ErrorCode.SomeUserErrorWithParameters, 
          "param1",
          "param2");

        // verify that it comes back in english
        Assert.AreEqual("{\"Text\":\"This is an error with parameters: {0}, {1}\",\"Arguments\":[\"param1\",\"param2\"]}", unLocalisedString);

        // get the localised translation
        string localisedString = translator.LocaliseUnlocalisedString(unLocalisedString);

        // verify that it comes back in french
        Assert.AreEqual("Ceci est une erreur avec les paramètres: param1, param2", localisedString);
      }
      finally
      {
        // reset current ui culture
        CultureInfo.CurrentUICulture = initialUICulture;
      }
    }

    [Test]
    public void GivenTranslations_GetLocales()
    {
      var locales = DataServices.Resolve<ILocalisedStringResourceCache>().GetLocales();
      
      Assert.NotNull(locales);

      // check for the current supported locales in the Language project
      Assert.Contains("en-gb", locales);
      Assert.Contains("fr-fr", locales);
      Assert.Contains("es-mx", locales);
      Assert.Contains("de-de", locales);
    }
  }
}