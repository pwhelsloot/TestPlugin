namespace TestDataGenerator
{
  using System;
  using System.IO;
  using System.Xml.Linq;
  using AMCS.Data;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.DataSets.Support;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.TestData;
  using log4net;

  public class Runner
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Runner));
    private readonly Arguments arguments;

    public Runner(Arguments arguments)
    {
      this.arguments = arguments;
    }
    public void Run()
    {
      if (!string.IsNullOrEmpty(arguments.ExportSchemaFilepath))
        ExportSchema();
      else
      {
        PerformServiceSetup();
        GenerateTestData();
      }
    }

    private void PerformServiceSetup()
    {
      var connectionString = ConnectionStringEncryption.Decrypt(arguments.ConnectionString);

      var configuration = XDocument.Load("configuration.xml");
      var _ = ServiceSetup.Setup(connectionString, configuration);
    }

    private void ExportSchema()
    {
      Logger.Info("Exporting schema");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.TestPlugin.Server.", "AMCS.Data.Server.");
      var jsonSchema = new TestDataService(serverTypes).GetJsonSchema();
      File.WriteAllText(arguments.ExportSchemaFilepath, jsonSchema);
      Logger.Info($"Schema exported successfully to: {arguments.ExportSchemaFilepath}");
    }

    private void GenerateTestData()
    {
      var sessionToken = DataServices.Resolve<IUserService>().SystemUserSessionKey;
      var testDataService = DataServices.Resolve<ITestDataService>();
      testDataService.GenerateTestData(arguments.ImportJsonFilepath, arguments.ImportSeed, sessionToken);
    }
  }
}
