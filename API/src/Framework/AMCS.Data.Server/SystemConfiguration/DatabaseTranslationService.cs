namespace AMCS.Data.Server.SystemConfiguration
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;
  using System.Resources;
  using AMCS.Data.Configuration;

  public class DatabaseTranslationService : IDatabaseTranslationsService
  {
    private const string DefaultLanguage = "en";
    private const string DbStringResourcePrefix = "AmcsDbStrings";
    private readonly Assembly languageAssembly;
    private readonly string languageNamespace;
    private Dictionary<string, string> translationResources;

    public DatabaseTranslationService(ILanguageResources languageResources)
    {
      this.languageAssembly = languageResources.Assembly;
      this.languageNamespace = languageResources.StringResourcesNamespace;
    }

    private Dictionary<string, string> GetApplicableResourceFiles()
    {
      var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      var languageFileNames = this.languageAssembly.GetManifestResourceNames();
      if (languageFileNames == null || languageFileNames.Length == 0)
      {
        return result;
      }

      string dbStringsResourcePrefix = $"{this.languageNamespace}.{DbStringResourcePrefix}.";
      var dbStringResources = Array.FindAll(languageFileNames, l => l.StartsWith(dbStringsResourcePrefix, StringComparison.OrdinalIgnoreCase));
      foreach (string dbStringResource in dbStringResources)
      {
        var languageCode = Path.GetFileNameWithoutExtension(dbStringResource.Substring(dbStringsResourcePrefix.Length));

        if (!string.IsNullOrEmpty(languageCode))
        {
          result.Add(languageCode, dbStringResource);
        }
      }

      return result;
    }

    public Dictionary<string, string> GetTranslations(string languageCode)
    {
      if(this.translationResources == null)
        this.translationResources = this.GetApplicableResourceFiles();

      translationResources.TryGetValue(languageCode, out var languageResource);
      if (string.IsNullOrEmpty(languageResource))
      {
        if (languageCode.Contains("-"))
          return GetTranslations(languageCode.Substring(0, languageCode.LastIndexOf('-')));

        //If we are defaulting to no translations ("en") returning empty instead of null
        // This means there is no translations loaded but it is a "valid" languageCode
        if (languageCode == DefaultLanguage)
          return new Dictionary<string, string>();

        return null;
      }

      var translations = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
      using (var stream = this.languageAssembly.GetManifestResourceStream(languageResource))
      {
        if (stream == null)
        {
          throw new InvalidOperationException($"The resource file '{languageResource}' could not be loaded.");
        }
        using (var reader = new ResourceReader(stream))
        {
          foreach (DictionaryEntry entry in reader)
          {
            translations.Add(entry.Key as string, entry.Value as string);
          }
        }
      }
      return translations;
    }
  }
}
