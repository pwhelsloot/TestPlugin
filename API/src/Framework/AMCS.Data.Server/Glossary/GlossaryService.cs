namespace AMCS.Data.Server.Glossary
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity;
  using AMCS.Data.Entity.Glossary;
  using AMCS.Data.Server.Api;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.WebHook;
  using AMCS.PluginData.Data.WebHook;
  using log4net;
  using Plugin;

  public class GlossaryService : IGlossaryService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GlossaryService));

    private readonly IUserService userService;
    private readonly IGlossaryCacheService glossaryCacheService;
    private readonly IWebHookService webhookService;
    private readonly IDictionary<string, string> cachedTranslations = new Dictionary<string, string>();

    private readonly IRestApiService restApiService;
    private readonly string coreServiceRoot;
    private readonly string tenantId;

    public GlossaryService(
      IUserService userService,
      IGlossaryCacheService glossaryCacheService,
      IRestApiService restApiService,
      ISetupService setupService,
      IWebHookService webhookService,
      string coreServiceRoot,
      string tenantId)
    {
      if (string.IsNullOrWhiteSpace(coreServiceRoot))
        throw new ArgumentNullException(nameof(coreServiceRoot));

      if (string.IsNullOrWhiteSpace(tenantId))
        throw new ArgumentNullException(nameof(tenantId));

      this.userService = userService;
      this.glossaryCacheService = glossaryCacheService;
      this.restApiService = restApiService;
      this.webhookService = webhookService;

      this.coreServiceRoot = coreServiceRoot.TrimEnd('/');
      this.tenantId = tenantId;

      setupService.RegisterSetup(RegisterGlossaryWebhook, 1000);
    }

    public IList<ApiGlossary> GetGlossaries()
    {
      return glossaryCacheService.GetGlossaries();
    }

    public string Translate(string input, string languageCode)
    {
      if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(languageCode))
        return input;

      var hash = $"{languageCode}:{input}";

      if (!cachedTranslations.ContainsKey(hash))
      {
        var glossaries = GetGlossaries();
        var result = GetCompoundLanguageCode(input, languageCode, glossaries);

        cachedTranslations.Add(hash, result);
      }

      return cachedTranslations[hash];
    }

    private static string GetCompoundLanguageCode(string input, string languageCode, IList<ApiGlossary> glossaries)
    {
      var translation = glossaries.FirstOrDefault(glossary =>
          glossary.Original.Equals(input, StringComparison.OrdinalIgnoreCase) &&
          glossary.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)
        );

      if (translation != null)
        return translation.Translated;

      return languageCode.Contains("-")
        ? GetCompoundLanguageCode(
          input,
          languageCode.Substring(0, languageCode.LastIndexOf("-", StringComparison.Ordinal)),
          glossaries)
        : input;
    }

    public void Start()
    {
      GetGlossariesFromCore();
    }

    private void RegisterGlossaryWebhook()
    {
      webhookService.Register($"{PluginHelper.GetCorePluginFullName()}:Glossary",
        WebHookFormat.Simple,
        WebHookTrigger.Save | WebHookTrigger.Delete,
        webHookCallback =>
        {
          GetGlossariesFromCore();
        },
        string.Empty);
    }

    private void GetGlossariesFromCore()
    {
      try
      {
        var systemToken = userService.CreateSystemSessionToken();
        using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
        using (var transaction = dataSession.CreateTransaction())
        {
          var apiResult = restApiService.GetCollection<ApiGlossary>(new RestApiService.GetCollectionParams
          {
            Username = "admin",
            UserRoles = { WellKnownPlatformRoles.Admin },
            TenantId = tenantId,
            Url = $"{coreServiceRoot}/services/api/glossary/GlossaryBrowsers"
          });

          var existingDbGlossaries = dataSession.GetAll<ApiGlossary>(systemToken, false);
          foreach (var dbGlossary in existingDbGlossaries)
          {
            dataSession.Delete(systemToken, dbGlossary, false);
          }

          foreach (var apiGlossary in apiResult.Resource)
          {
            apiGlossary.GlossaryInternalCacheId = null;
          }

          glossaryCacheService.Publish(apiResult.Resource, null, systemToken, dataSession);
          transaction.Commit();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Could not get glossaries from Core", ex);
      }
    }
  }
}