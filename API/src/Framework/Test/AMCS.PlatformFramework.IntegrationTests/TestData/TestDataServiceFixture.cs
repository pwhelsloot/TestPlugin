namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using System.Collections.Generic;
  using System.IO;
  using AMCS.Data;
  using AMCS.Data.Server.TestData;
  using AMCS.PlatformFramework.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class TestDataServiceFixture : TestBase
  {
    [Test]
    public void GenerateJsonSchema()
    {
      var schema = DataServices
          .Resolve<ITestDataService>()
          .GetJsonSchema();
      Assert.IsNotNull(schema);
    }


    [Test]
    public void ValidateValidJsonInputAgainstSchema()
    {
      var jsonInput = validJsonInput;
      var testDataService = DataServices.Resolve<ITestDataService>();
      IList<string> errorMessages;
      testDataService.IsValidJson(jsonInput, out errorMessages);
      Assert.That(errorMessages?.Count == 0);
    }

    [Test]
    public void ValidateInvalidJsonInputAgainstSchema()
    {
      var jsonInput = invalidJsonInput;
      var testDataService = DataServices.Resolve<ITestDataService>();
      IList<string> errorMessages;
      testDataService.IsValidJson(jsonInput, out errorMessages);
      Assert.That(errorMessages?.Count > 0);
    }

    [Test]
    public void ExtractUserOptionsFromJsonInput()
    {
      var jsonInput = validJsonInput;
      var testDataService = DataServices.Resolve<ITestDataService>();
      Assert.IsTrue(testDataService.IsValidJson(jsonInput, out _));
      var converted = testDataService.GetOptions<UserTestDataOptions>(jsonInput);
      Assert.IsNotNull(converted);
      Assert.AreEqual(10, converted.Number);
      Assert.AreEqual("amcstestdata.com", converted.EmailDomain);
    }

    [Test]
    public void ExtractUserOptionsFromJsonFile()
    {
      var tempFile = Path.GetTempFileName();
      File.WriteAllText(tempFile, validJsonInput);
      var jsonInput = validJsonInput;
      var testDataService = DataServices.Resolve<ITestDataService>();
      Assert.IsTrue(testDataService.IsValidJson(jsonInput, out _));
      var converted = testDataService.GetOptions<UserTestDataOptions>(jsonInput);
      Assert.IsNotNull(converted);
      Assert.AreEqual(10, converted.Number);
      Assert.AreEqual("amcstestdata.com", converted.EmailDomain);
    }

    [Test]
    public void ExtractSysConfigOptionsFromJsonInput()
    {
      var jsonInput = validJsonInput;
      var testDataService = DataServices.Resolve<ITestDataService>();
      Assert.IsTrue(testDataService.IsValidJson(jsonInput, out _));
      var converted = testDataService.GetOptions<SystemConfigurationTestDataOptions>(jsonInput);
      Assert.IsNotNull(converted);
      Assert.AreEqual(12, converted.Number);
    }

    private readonly string validJsonInput = @"
        {
          'RecordTypes' : {
            'User' : {
              'Options' : {
                'Number' : 10,
                'EmailDomain' : 'amcstestdata.com'
              }
            },
            'SystemConfiguration' : {
              'Options' : {
                'Number' : 12
              }
            },
          }
        }";

    private readonly string invalidJsonInput = @"
        {
          'RecordTypes' : {
            'UserTestDataOptions' : {
              'Options' : {
                'NumberOfUsers' : 10,
                'EmailDomain' : 'amcstestdata.com',
                'AdditionalProperty' : 123
              }
            },
            'SystemConfigOptions' : {
              'Options' : {
                'Number' : 10
              }
            },
          }
        }";
  }  
}
