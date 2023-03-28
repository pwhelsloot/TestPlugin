//-----------------------------------------------------------------------------
// <copyright file="LocalisedStringResourceCache.cs" company="AMCS Group">
//   Copyright © 2010-16 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Resource
{
  using System;
  using System.Collections;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Resources;

  public class LocalisedStringResourceCache : ILocalisedStringResourceCache
  {
    private enum StringGrade
    {
      MatchLocale,
      MatchLanguage,
      MatchLanguageNotRegion,
      Default,
      NoMatch
    }

    private const string DefaultLanguage = "en";
    private const string DefaultLocale = "en-gb";
    private const string StringResourceFileExtension = ".resx";
    private const string DefaultStringResourceFileName = "AmcsStrings";

    private readonly IDictionary<string, IDictionary<string, string>> strings = new Dictionary<string, IDictionary<string, string>>();
    private readonly List<string> locales = new List<string>(); 
    private readonly Assembly languageAssembly;
    private readonly string languageNamespace;

    internal LocalisedStringResourceCache(ILanguageResources languageResources)
    {
      this.languageAssembly = languageResources.Assembly;
      this.languageNamespace = languageResources.StringResourcesNamespace;

      var resourceFiles = this.GetApplicableResourceFiles();
      foreach (var resourceFile in resourceFiles)
      {
        var locale = resourceFile.Value.ToLower();
        this.ReadResourceFile(resourceFile.Key, locale);

        if (!locales.Contains(locale))
          locales.Add(locale);
      }
    }

    private IDictionary<string, string> GetApplicableResourceFiles()
    {
      var result = new Dictionary<string, string>();

      var languageFileNames = this.languageAssembly.GetManifestResourceNames();
      if (languageFileNames == null || languageFileNames.Length == 0)
      {
        throw new FileNotFoundException($"No string resource files found in directory '{this.languageAssembly.Location}'");
      }

      var resxFiles = Array.FindAll(languageFileNames, l => l.Contains(DefaultStringResourceFileName));
      if (resxFiles.Length == 0)
      {
        throw new FileNotFoundException($"No string resource files found in directory '{this.languageAssembly.Location}'");
      }

      foreach (string resxFile in resxFiles)
      {
        string toRemove = $"{this.languageNamespace}.";
        var filename = Path.GetFileNameWithoutExtension(resxFile.StartsWith(toRemove) ? resxFile.Substring(resxFile.IndexOf(toRemove, StringComparison.InvariantCultureIgnoreCase) + toRemove.Length) : resxFile);

        if (!string.Equals(filename, DefaultStringResourceFileName, StringComparison.OrdinalIgnoreCase))
        {
          int lastDotIndex = filename.LastIndexOf('.');
          if (lastDotIndex > -1)
          {
            var fileLocale = filename.Substring(lastDotIndex + 1).ToLower();
            result.Add(resxFile, fileLocale);
          }
          else
          {
            throw new InvalidOperationException($"The resource file '{resxFile}' is not named appropriately.  Expect string resource files to have names like '{DefaultStringResourceFileName}'.es-es{StringResourceFileExtension}'");
          }
        }
        else
        {
          result.Add(resxFile, DefaultLocale);
        }
      }

      return result;
    }

    private void ReadResourceFile(string fileName, string locale)
    {
      var stream = this.languageAssembly.GetManifestResourceStream(fileName);
      if (stream == null)
      {
        throw new InvalidOperationException($"The resource file '{fileName}' could not be loaded.");
      }

      using (var reader = new ResourceReader(stream))
      {
        foreach (DictionaryEntry entry in reader)
        {
          string key = entry.Key as string;
          string value = entry.Value as string;
          if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
          {
            if (!this.strings.ContainsKey(key))
            {
              this.strings.Add(key, new ConcurrentDictionary<string, string>());
            }

            if (!this.strings[key].ContainsKey(locale))
            {
              value = value.TrimEnd('\r', '\n');
              this.strings[key].Add(locale, value);
            }
          }
        }
      }
    }

    /// <summary>
    /// If a reasonable quality match is found for key 'primaryKey' then it is returned.  A reasonable match is classed as
    /// at worst a string that's matched on the current language but the region is different.
    /// If a reasonable quality match is not found then a second attempt is made using the "fallbackKey".  The best quality 
    /// result between these two keys is returned.
    /// </summary>
    /// <param name="primaryKey"></param>
    /// <param name="fallbackKey"></param>
    /// <returns></returns>
    public string GetString(string primaryKey, string fallbackKey)
    {
      Tuple<StringGrade, string> gradedString = this.GetGradedString(primaryKey);
      if (gradedString.Item1 >= StringGrade.Default)
      {
        Tuple<StringGrade, string> fallbackAttempt = this.GetGradedString(fallbackKey);
        if (fallbackAttempt.Item1 < gradedString.Item1)
        {
          gradedString = fallbackAttempt;
        }
      }

      return gradedString.Item2;
    }

    public List<(string, string)> GetStrings(string primaryKey)
    {
      var translations = new List<(string, string)>();

      if (strings.TryGetValue(primaryKey, out var results))
        translations = results.Select(r => (r.Key, r.Value)).ToList();

      if (translations.Count == 0)
        translations.Add(new ValueTuple<string, string>(DefaultLocale, primaryKey));
      
      return translations;
    }

    public List<string> GetLocales()
    {
      if (locales.Count == 0)
        locales.Add(DefaultLocale);

      return locales;
    }

    /// <summary>
    /// Returns a string and it's grade based on a key.  The quality of the translation is Item1 in the Tuple result.
    /// Grading levels are:
    /// MatchLocale: Exact locale match, e.g. "es-es"
    /// MatchLanguage: Match on language where the resource is not region specific, e.g. the resource locale is just "es"
    /// MatchLanguageNotRegion: Match on language where the resource is specific to any region, e.g. the resource locale could be "es-mx"
    /// Default: String for the default locale is returned, i.e. will be in English
    /// NoMatch: No match
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private Tuple<StringGrade, string> GetGradedString(string key)
    {
      var locale = CultureInfo.CurrentUICulture.Name.ToLower();
      var language = this.SplitLocale(locale).Language.ToLower();

      string result = null;
      StringGrade grade = StringGrade.NoMatch;
      if (this.strings.ContainsKey(key))
      {
        if (this.strings[key].ContainsKey(locale))
        {
          result = this.strings[key][locale];
          grade = StringGrade.MatchLocale;
        }
        else if (this.strings[key].ContainsKey(language))
        {
          result = this.strings[key][language];
          grade = StringGrade.MatchLanguage;
        }
        else
        {
          string altRegionKey = null;
          if (language != DefaultLanguage)
          {
            altRegionKey = this.strings[key].Keys.FirstOrDefault(k => k.StartsWith($"{language}-"));
          }

          if (!string.IsNullOrWhiteSpace(altRegionKey))
          {
            result = this.strings[key][altRegionKey];
            grade = StringGrade.MatchLanguageNotRegion;
          }
          else if (this.strings[key].ContainsKey(DefaultLocale))
          {
            result = this.strings[key][DefaultLocale];
            grade = StringGrade.Default;
          }
        }
      }

      return new Tuple<StringGrade, string>(grade, result);
    }

    /// <summary>
    /// result.Item1 = language, result.Item2 = region.  Region may be null
    /// </summary>
    /// <param name="locale"></param>
    /// <returns></returns>
    private (string Language, string Region) SplitLocale(string locale)
    {
      string[] parts = locale.Split('-');
      if (parts.Length == 1)
      {
        return (parts[0], null);
      }

      if (parts.Length == 2)
      {
        return (parts[0], parts[1]);
      }

      throw new FormatException("Invalid language file format.");
    }
  }
}
