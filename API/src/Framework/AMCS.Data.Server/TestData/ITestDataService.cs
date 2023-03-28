namespace AMCS.Data.Server.TestData
{
  using System.Collections.Generic;

  public interface ITestDataService
  {
    string GetJsonSchema();

    bool IsValidJson(string json, out IList<string> errorMessages);

    T GetOptions<T>(string json);
    T GetConfiguration<T>(string json);

    void GenerateTestData(string jsonFilePath, string seedValue, ISessionToken sessionToken);
  }
}
