//-----------------------------------------------------------------------------
// <copyright file="Translator.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.Translate
{
  using System.Globalization;
  using System.Linq;

  public abstract class Translator
  {
    /// <summary>
    /// If there is no exact mapping for a locale code then fallback to a language match and then if still nothing fallback to the default.
    /// </summary>
    /// <param name="localeCode">The locale code.</param>
    /// <param name="localeString">The locale string.</param>
    /// <returns></returns>
    protected ILocaleString GetFallbackString(string localeCode, ITranslatableLocaleString localeString)
    {
      string languageCode = localeCode;
      if (languageCode.Length > 2)
      {
        languageCode = languageCode.Substring(0, 2);
      }

      ILocaleString translation = null;

      // Don't want to go in here if the language code is "en", just jump past and get the default value.
      if (languageCode != "en" && localeString.Translations != null && localeString.Translations.Count > 0)
      {
        // Look for a straight match on language
        translation = localeString.Translations.FirstOrDefault(t => t.Locale.Equals(languageCode));

        // If nothing found just for language then pick one that is for the language - may not be a good region though, e.g. "ie" v's "us"
        if (translation == null || string.IsNullOrWhiteSpace(translation.Value))
        {
          translation = localeString.Translations.FirstOrDefault(t => t.Locale.StartsWith(languageCode));
        }
      }
      
      if (translation == null || string.IsNullOrWhiteSpace(translation.Value))
      {
        translation = new LocaleString();
        translation.Locale = CultureInfo.CurrentUICulture.Name;
        translation.Value = localeString.Value; // Just give it the default value
      }

      return translation;
    }
  }
}