namespace TestDataGenerator
{
  using CommandLine;

  public class Arguments
  {
    [Option("export-schema-filepath", HelpText = "Absolute filepath to export Json shema to", SetName = "Export", Required = true)]
    public string ExportSchemaFilepath { get; set; }

    [Option("connection-string", HelpText = "Connection string for TestPlugin database", SetName = "Import", Required = true)]
    public string ConnectionString { get; set; }

    [Option("import-json-filepath", HelpText = "Absolute filepath to import Json from", SetName = "Import", Required = true)]
    public string ImportJsonFilepath { get; set; }

    [Option("import-seed", HelpText = "Seed value for use when generating import data", SetName = "Import", Required = true)]
    public string ImportSeed { get; set; }
  }
}
