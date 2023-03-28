#pragma warning disable CS0618
namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.DataSets.Support;
  using log4net;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Newtonsoft.Json.Schema;

  public class TestDataService : ITestDataService
  {
    private const string JsonSchemaTitle = "TestDataGeneratorSchema";
    private const string RecordTypesProperty = "RecordTypes";
    private const string RecordTypeOptionsProperty = "Options";
    private const string RecordTypeDependenciesProperty = "Dependencies";

    private static readonly ILog Logger = LogManager.GetLogger(typeof(TestDataService));

    private readonly TestDataConfigurationContainer Configuration;
    private readonly List<TestDataOptionsContainer> Options = new List<TestDataOptionsContainer>();
    private readonly JsonSchema JsonSchema;

    public TestDataService(TypeManager typeManager)
    {
      TestDataHelper.Extract(typeManager, out Type configurationType, out List<Type> optionsTypes);
      Configuration = new TestDataConfigurationContainer(configurationType);
      optionsTypes.ForEach(optionType => Options.Add(new TestDataOptionsContainer(optionType)));
      this.JsonSchema = GenerateJsonSchema();
    }

    private JsonSchema GenerateJsonSchema()
    {
      void SetSchemaBehaviour(JsonSchema jsonSchema)
      {
        jsonSchema.AllowAdditionalItems = false;
        jsonSchema.AllowAdditionalProperties = false;
        foreach (var property in jsonSchema.Properties)
        {
          property.Value.Required = false;
        }
      }

      JsonSchema EmptySchema(JsonSchemaGenerator jsGenerator) => jsGenerator.Generate((new { }).GetType());

      var jsSchemaGenerator = new JsonSchemaGenerator() { };
      var schema = EmptySchema(jsSchemaGenerator);
      schema.Title = JsonSchemaTitle;
      schema.Properties.Add(Configuration.DisplayName, jsSchemaGenerator.Generate(Configuration.ConfigurationType));
      SetSchemaBehaviour(schema);
      var recordTypesSchema = EmptySchema(jsSchemaGenerator);
      schema.Properties.Add(RecordTypesProperty, recordTypesSchema);
      foreach (var testDataStruct in Options)
      {
        schema.Properties[RecordTypesProperty].Properties.Add(testDataStruct.DisplayName, EmptySchema(jsSchemaGenerator));
        var optionsSchema = jsSchemaGenerator.Generate(testDataStruct.TestDataOptions);
        SetSchemaBehaviour(optionsSchema);
        schema.Properties[RecordTypesProperty].Properties[testDataStruct.DisplayName].Properties.Add(RecordTypeOptionsProperty, optionsSchema);
        if (testDataStruct.DependsOnTypes.Count >0)
        {
          var dependenciesSchema = EmptySchema(jsSchemaGenerator);

          foreach (var dependency in testDataStruct.DependsOnTypes)
          {
            var dependantStruct = Options.Single(tds => tds.RecordType == dependency);
            dependenciesSchema.Properties.Add(dependantStruct.DisplayName, EmptySchema(jsSchemaGenerator));
          }
          schema.Properties[RecordTypesProperty].Properties[testDataStruct.DisplayName].Properties.Add(RecordTypeDependenciesProperty, dependenciesSchema);
        }

      }

      return schema;
    }

    public T GetOptions<T>(string json)
    {
      return (T)GetOptions(json, typeof(T));
    }

    private object GetOptions(string json, Type optionType)
    {
      var optionsAttribute = optionType.GetCustomAttribute<TestDataOptionsAttribute>();
      var jobj = JObject.Parse(json);
      var jOptions = jobj?[RecordTypesProperty]?[optionsAttribute.DisplayName]?[RecordTypeOptionsProperty];
      var optionsString = jOptions?.ToString() ?? string.Empty;
      return JsonConvert.DeserializeObject(optionsString, optionType);
    }

    public T GetConfiguration<T>(string json)
    {
      return (T)GetConfiguration(json);
    }

    private object GetConfiguration(string json)
    {
      var jobj = JObject.Parse(json);
      var jConfig = jobj[Configuration.DisplayName];
      var configString = jConfig?.ToString() ?? string.Empty;
      var result = JsonConvert.DeserializeObject(configString, Configuration.ConfigurationType);
      if (result == null)
        result = Activator.CreateInstance(Configuration.ConfigurationType);
      return result;
    }

    public string GetJsonSchema()
    {
      return JsonSchema.ToString();
    }

    public bool IsValidJson(string input, out IList<string> errorMessages)
    {
      var inputObject = JObject.Parse(input);
      return inputObject.IsValid(JsonSchema, out errorMessages);
    }

    private string LoadImportFile(string filePath)
    {
      string json = File.ReadAllText(filePath);
      if (IsValidJson(json, out IList<string> errorMessages))
        return json;
      else
        throw new ArgumentException(string.Join(",", errorMessages));
    }

    public void GenerateTestData(string jsonFilePath, string seedValue, ISessionToken sessionToken)
    {
      Logger.Info("Generating Test Data");
      var loadedJson = LoadImportFile(jsonFilePath);
      ValidateDependenciesProvided(loadedJson);
      var builder = new TestDataBuilder(seedValue);
      var dataSession = BslDataSessionFactory.GetDataSession(sessionToken);
      
      var config = GetConfiguration(loadedJson);
      foreach (var testDataStruct in Options)
      {
        var options = GetOptions(loadedJson, testDataStruct.TestDataOptions);
        if (options != null)
        {
          Logger.Info($"Generating data from {testDataStruct.DisplayName}");
          var generator = DataServices.Resolve(typeof(ITestDataGenerator<,>).MakeGenericType(Configuration.ConfigurationType, testDataStruct.TestDataOptions));
          var generatorCaller = (IGeneratorCaller)Activator.CreateInstance(typeof(GeneratorCaller<,>).MakeGenericType(Configuration.ConfigurationType, testDataStruct.TestDataOptions), generator);
          generatorCaller.Generate(builder, config, options, sessionToken, dataSession);
        }
      }

      Logger.Info("Performing import");
      var result = new DataSetImportResultReader(DataServices.Resolve<IDataSetService>().Import(sessionToken, builder.Build()));
      LogImportErrorMessages(result);
      Logger.Info("Import completed");
    }

    private void ValidateDependenciesProvided(string json)
    {
      foreach (var testDataStruct in Options)
      {
        var options = GetOptions(json, testDataStruct.TestDataOptions);
        if (options != null && testDataStruct.DependsOnTypes.Any())
        {
          // Options provided for this type, and it has dependencies, so need to check that options provided for all dependencies
          foreach (var dependency in testDataStruct.DependsOnTypes)
          {
            var dependantStruct = Options.Single(option => option.RecordType == dependency);
            var dependencyOptions = GetOptions(json, dependantStruct.TestDataOptions);
            if(dependencyOptions == null)
              throw new ArgumentException($"{testDataStruct.DisplayName} depends on {dependantStruct.DisplayName} which was not provided");
          }
        }
      }
    }

    private void LogImportErrorMessages(DataSetImportResultReader result)
    {
      if (result.Result.Messages.HasErrors)
      {
        Logger.Error($"{result.FailedRecords.Count} errors encountered during import");
        foreach (var message in result.Result.Messages.Where(message => message.Type == DataSets.Import.MessageType.Error))
        {
          Logger.Error(message.Description);
        }
      }
    }

    private class GeneratorCaller<TConfig, TOptions> : IGeneratorCaller
    {
      private readonly ITestDataGenerator<TConfig, TOptions> generator;

      public GeneratorCaller(ITestDataGenerator<TConfig, TOptions> generator)
      {
        this.generator = generator;
      }

      public void Generate(TestDataBuilder builder, object config, object options, ISessionToken sessionToken, IDataSession dataSession)
      {
        generator.Generate(builder, (TConfig)config, (TOptions)options, sessionToken, dataSession);
      }
    }

    private interface IGeneratorCaller
    {
      void Generate(TestDataBuilder builder, object config, object optionSettings, ISessionToken sessionToken, IDataSession dataSession);
    }
  }
}
