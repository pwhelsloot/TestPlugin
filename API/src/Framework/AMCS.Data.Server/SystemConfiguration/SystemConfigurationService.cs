namespace AMCS.Data.Server.SystemConfiguration
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Configuration;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.SQL.Querying;
  using log4net;

  public class SystemConfigurationService : ISystemConfigurationService, IDelayedStartup
  {
    public const string SystemConfigurationNamespace = "https://schemas.amcsgroup.io/platform/system-configuration";
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SystemConfigurationService));
    private static readonly XNamespace XsiNs = "http://www.w3.org/2001/XMLSchema-instance";

    private readonly Type configurationType;
    private readonly XmlSerializer serializer;
    private readonly Dictionary<Type, IElement> configurationElements = new Dictionary<Type, IElement>();

    public string XsdSchema { get; }

    public SystemConfigurationService(Type configurationType)
    {
      this.configurationType = configurationType;

      if (!typeof(IConfiguration).IsAssignableFrom(configurationType))
        throw new InvalidOperationException($"Type '{configurationType}' does not implement '{nameof(IConfiguration)}'");

      serializer = new XmlSerializer(configurationType);

      XsdSchema = GenerateSchema();

      LoadConfigurationElements();
    }

    private void LoadConfigurationElements()
    {
      var configuration = (IConfiguration)Activator.CreateInstance(configurationType);

      foreach (var collection in configuration.GetCollections())
      {
        var elementType = typeof(Element<>).MakeGenericType(collection.GetItemType());

        configurationElements.Add(collection.GetItemType(), (IElement)Activator.CreateInstance(elementType));
      }
    }

    private string GenerateSchema()
    {
      var schemas = new XmlSchemas();

      var mapping = new XmlReflectionImporter().ImportTypeMapping(configurationType);
      new XmlSchemaExporter(schemas).ExportTypeMapping(mapping);

      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream))
        {
          foreach (XmlSchema schema in schemas)
          {
            schema.Write(writer);
          }
        }

        return Encoding.UTF8.GetString(stream.ToArray());
      }
    }

    public IExportResult LoadConfiguration(ISessionToken userId)
    {
      var configuration = (IConfiguration)Activator.CreateInstance(configurationType);
      using (var dataSession = BslDataSessionFactory.GetDataSession(userId))
      using (var transaction = dataSession.CreateTransaction())
      {
        DbSystemConfigurationEntity profile = GetSystemConfiguration(userId, dataSession, DbSystemConfigurationConstants.SystemConfigurationProfileKey);

        if (profile == null)
        {
          return new ExportResultFailure($"Could not find DBSystemConfiguration key {DbSystemConfigurationConstants.SystemConfigurationProfileKey}");
        }

        configuration.ProfileName = profile.Key;

        foreach (var collection in configuration.GetCollections())
        {
          var element = configurationElements[collection.GetItemType()];

          collection.SetItems(element.GetAll(userId, dataSession));
        }

        transaction.Commit();
      }

      var xml = ConfigurationToXml(configuration);

      return new ExportResultSuccess(xml);
    }

    public ISaveResult SaveConfiguration(ISessionToken userId, string xmlBase, List<string> xmlOverrides = null, string databaseLanguage = "")
    {
      IConfiguration configuration;

      // Get the base config
      using (var reader = new StringReader(xmlBase))
      {
        configuration = (IConfiguration)serializer.Deserialize(reader);
      }

      // Build a list of overrides
      if (xmlOverrides != null)
      {
        foreach (string xmlOverride in xmlOverrides)
        {
          using (var reader = new StringReader(xmlOverride))
          {
            configuration.Merge((IConfiguration)serializer.Deserialize(reader));
          }
        }
      }

      // Deduplicate
      var configValidationResult = this.ValidateConfiguration(configuration);
      switch (configValidationResult)
      {
        case ValidationResultSuccess _:
          Logger.Info("System Configuration Profile Validated Successfully");
          break;
        case ValidationResultFailure validationResultFailure:
          string errorMessage = $"System Configuration Profile Validation Failed: {validationResultFailure.Message}";
          Logger.Error(errorMessage);
          return new SaveResultValidationFailure(errorMessage);
      }

      using (var dataSession = BslDataSessionFactory.GetDataSession(userId))
      using (var transaction = dataSession.CreateTransaction())
      {
        var translations = GetTranslations(userId, dataSession, databaseLanguage, out string translationsErrorMessage);
        if (!string.IsNullOrEmpty(translationsErrorMessage))
        {
          return new SaveResultValidationFailure(translationsErrorMessage);
        }

        var profileValidation = this.Validate(userId, dataSession, configuration.ProfileName);
        if (!profileValidation.IsValid)
        {
          return new SaveResultValidationFailure(profileValidation.ErrorMessage);
        }

        bool importErrors = false;


        foreach (var collection in configuration.GetCollections())
        {
          var items = collection.GetItems();

          if (items.Count <= 0 && collection.Transform != Transform.Replace)
          {
            Logger.Info($"Not saving {collection.GetItemType().Name} because there are no items provided");
            continue;
          }

          Logger.Info($"Saving {items.Count} items of {collection.GetItemType().Name}");

          if (translations != null)
          {
            foreach (ConfigurationElement item in items)
              item.Translate(translations);
          }

          var element = configurationElements[collection.GetItemType()];

          element.Save(userId, items, collection.Transform, dataSession);
          var firstElementWithError = items.Cast<ConfigurationElement>().FirstOrDefault(i => i.ImportError != null);
          if (firstElementWithError != null)
          {
            Logger.Error($"Failed to save element with the following error: {firstElementWithError.ImportError}");
            importErrors = true;
          }
        }

        if (importErrors)
        {
          return new SaveResultImportFailure(ConfigurationToXml(configuration));
        }

        // The pattern for CreateTransaction is that the call to Commit is the last line in the using.
        // Preferred is to move the return out of the using to keep this pattern in tact.
        // The early exit above as such would also be prohibited to make sure you don't accidentally skip the Commit call.
        // However, that's what we are trying to accomplish here so we ignore the pattern for this specific case
        transaction.Commit();
        return new SaveResultSuccess();
      }
    }

    public IValidationResult ValidateConfiguration(IConfiguration configuration)
    {
      var validationFailureMessages = new List<string>();

      foreach (var collection in configuration.GetCollections())
      {
        if (collection.GetItems()?.Count <= 0 && collection.Transform != Transform.Replace)
          continue;

        var element = configurationElements[collection.GetItemType()];

        validationFailureMessages.AddRange(element.Validate(collection.GetItems()));
      }

      if (validationFailureMessages.Any())
        return new ValidationResultFailure(string.Join(Environment.NewLine, validationFailureMessages));
      else
        return new ValidationResultSuccess();
    }

    private Dictionary<string, string> GetTranslations(ISessionToken userId, IDataSession dataSession, string databaseLanguage, out string errorMessage)
    {
      errorMessage = string.Empty;
      var currentDatabaseLanguage = GetSystemConfiguration(userId, dataSession, DbSystemConfigurationConstants.DatabaseLanguage);

      // DB already translated so apply this language to upgrade
      if (currentDatabaseLanguage != null)
      {
        // No dblanguage param passed so apply existing
        if (string.IsNullOrEmpty(databaseLanguage))
          databaseLanguage = currentDatabaseLanguage.Value;

        // Doesn't match provided parameter
        if (!currentDatabaseLanguage.Value.Equals(databaseLanguage, StringComparison.OrdinalIgnoreCase))
        {
          errorMessage = $"Provided database language [{databaseLanguage}] does not match current database language [{currentDatabaseLanguage.Value}]";
          Logger.Error(errorMessage);
          return null;
        }
      }

      // At this point if not assigned we are not translating
      if (string.IsNullOrEmpty(databaseLanguage))
      {
        return null;
      }
      else
      {
        if (currentDatabaseLanguage == null)
        {
          // Saving now but transaction won't commit unless successful, will be rolled back if any issues applying confi profile
          SaveDbLanguage(userId, dataSession, databaseLanguage);
        }

        var translations = DataServices.Resolve<IDatabaseTranslationsService>().GetTranslations(databaseLanguage);
        if (translations == null)
        {
          errorMessage = $"No translations found for [{databaseLanguage}]";
          Logger.Error(errorMessage);
          return null;
        }

        return translations;
      }
    }

    private void SaveDbLanguage(ISessionToken userId, IDataSession dataSession, string databaseLanguage)
    {
      var dbLanguage = new DbSystemConfigurationEntity
      {
        Key = DbSystemConfigurationConstants.DatabaseLanguage,
        Value = databaseLanguage
      };

      dataSession.Save(userId, dbLanguage);
    }

    private string ConfigurationToXml(IConfiguration configuration)
    {
      var stream = new MemoryStream();

      using (var writer = XmlWriter.Create(stream))
      {
        serializer.Serialize(writer, configuration);
      }

      stream.Position = 0;

      var document = XDocument.Load(stream);
      document.Root.Add(new XAttribute(XsiNs + "schemaLocation", $"{SystemConfigurationNamespace} SystemConfiguration.xsd"));

      using (var outputStream = new MemoryStream())
      {
        using (var writer = new StreamWriter(outputStream))
        {
          document.Save(writer);
        }

        return Encoding.UTF8.GetString(outputStream.ToArray());
      }
    }

    private DbSystemConfigurationEntity GetSystemConfiguration(ISessionToken userId, IDataSession session, string configurationKey)
    {
      var criteria = Criteria.For(typeof(DbSystemConfigurationEntity))
        .Add(Expression.Eq(nameof(DbSystemConfigurationEntity.Key), configurationKey));

      var query = session.GetAllByCriteria<DbSystemConfigurationEntity>(userId, criteria);
      DbSystemConfigurationEntity result = query.SingleOrDefault();

      return result;
    }

    private DbSystemConfigurationEntity CreateProfile(ISessionToken userId, IDataSession session, string profileName)
    {
      var result = new DbSystemConfigurationEntity
      {
        Key = DbSystemConfigurationConstants.SystemConfigurationProfileKey,
        Value = profileName
      };

      result.DbSystemConfigurationId = session.Save(userId, result);

      return result;
    }

    private void SaveCoreVersion()
    {
      var userId = DataServices.Resolve<IUserService>().CreateSystemSessionToken();
      var session = BslDataSessionFactory.GetDataSession(userId);
      string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

      using (var transaction = session.CreateTransaction())
      {
        var criteria = Criteria.For(typeof(DbSystemConfigurationEntity))
        .Add(Expression.Eq(nameof(DbSystemConfigurationEntity.Key), DbSystemConfigurationConstants.LatestBackendVersionKey));

        var query = session.GetAllByCriteria<DbSystemConfigurationEntity>(userId, criteria);
        DbSystemConfigurationEntity versionConfiguration = query.SingleOrDefault();

        if (versionConfiguration != null)
        {
          versionConfiguration.Value = version;
        }
        else
        {
          versionConfiguration = new DbSystemConfigurationEntity
          {
            Key = DbSystemConfigurationConstants.LatestBackendVersionKey,
            Value = version
          };
        }

        session.Save(userId, versionConfiguration);
        transaction.Commit();
      }
    }

    private (bool IsValid, string ErrorMessage) Validate(ISessionToken userId, IDataSession session, string profileName)
    {
      if (string.IsNullOrEmpty(profileName))
      {
        return (false, "No profile supplied with configuration");
      }

      var profile = GetSystemConfiguration(userId, session, DbSystemConfigurationConstants.SystemConfigurationProfileKey);

      if (profile == null)
      {
        profile = CreateProfile(userId, session, profileName);
      }

      var matchingProfile = profile?.Value == profileName;

      return (matchingProfile, matchingProfile ? null : $"Invalid profile specified. Expected profile {profile?.Value}");
    }

    public void Start()
    {
      SaveCoreVersion();
    }

    private interface IElement
    {
      IList GetAll(ISessionToken userId, IDataSession dataSession);

      void Save(ISessionToken userId, IList elements, Transform transform, IDataSession dataSession);

      IList<string> Validate(IList elements);
    }

    private class Element<T> : IElement
    {
      private readonly IConfigurationSynchronizer<T> synchronizer;

      public Element()
      {
        var elementAttribute = typeof(T).GetCustomAttribute<ConfigurationElementAttribute>();
        if (elementAttribute == null)
          throw new InvalidOperationException($"Configuration type '{typeof(T)}' does not declare a [ConfigurationElement] attribute");

        var synchronizer = Activator.CreateInstance(elementAttribute.Synchronizer);
        if (!(synchronizer is IConfigurationSynchronizer<T>))
          throw new InvalidOperationException($"Synchronizer '{elementAttribute.Synchronizer}' of item type '{typeof(T)}' must implement '{typeof(IConfigurationSynchronizer<T>)}'");

        this.synchronizer = (IConfigurationSynchronizer<T>)synchronizer;
      }

      public IList GetAll(ISessionToken userId, IDataSession dataSession)
      {
        return (IList)synchronizer.GetAll(userId, dataSession);
      }

      public void Save(ISessionToken userId, IList elements, Transform transform, IDataSession dataSession)
      {
        if (!(elements is IList<T> typedElements))
          typedElements = elements.Cast<T>().ToList();

        synchronizer.Save(userId, typedElements, transform, dataSession);
      }

      public IList<string> Validate(IList elements)
      {
        if (!(elements is IList<T> typedElements))
          typedElements = elements.Cast<T>().ToList();

        return synchronizer.Validate(typedElements);
      }
    }
  }
}
