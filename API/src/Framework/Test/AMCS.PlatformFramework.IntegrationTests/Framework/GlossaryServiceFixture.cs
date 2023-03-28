namespace AMCS.PlatformFramework.IntegrationTests.Framework
{
  using System;
  using AMCS.Data;
  using AMCS.Data.Server.Api;
  using AMCS.Data.Server.Services;
  using System.Diagnostics;
  using System.Collections.Generic;
  using AMCS.Data.Entity.Glossary;
  using AMCS.Data.Server.Glossary;
  using Data.Configuration;
  using Data.Server.WebHook;
  using Moq;
  using NUnit.Framework;
  
  [TestFixture]
  public class GlossaryServiceFixture : TestBase
  {
    [Test]
    public void GivenCompoundLanguageCode_WhenTranslating_ThenGetTranslated()
    {
      var glossaryService = CreateGlossaryService();

      var actual = glossaryService.Translate("Traffic Light", "en-za-us-au-gb");
      Assert.AreEqual("Robot", actual);
    }
   
    [Test]
    public void GivenRepeatingInputs_WhenTranslating_ThenGetCachedTranslations()
    {
      var glossaryService = CreateGlossaryService();
      var stopWatch = new Stopwatch();
      
      stopWatch.Start();
      var firstActual = glossaryService.Translate("Traffic Light", "en-za-us-au-gb");
      var firstRun = stopWatch.ElapsedTicks;
      
      stopWatch.Restart();
      var secondActual = glossaryService.Translate("Traffic Light", "en-za-us-au-gb");
      var secondRun = stopWatch.ElapsedTicks;
      stopWatch.Stop();
      
      Assert.AreEqual("Robot", firstActual);
      Assert.AreEqual("Robot", secondActual);
      Assert.IsTrue(firstRun > secondRun, "The cache didn't work as the cached run took longer");
    }
    
    [Test]
    public void GivenCompoundLanguageCode_WhenGettingRootTranslation_ThenGetTranslated()
    {
      var glossaryService = CreateGlossaryService();

      var actual = glossaryService.Translate("Football", "en-za");
      Assert.AreEqual("Soccer", actual);
    }
    
    [Test]
    public void GivenInput_WhenNoGlossaryAvailable_ThenGetInput()
    {
      var glossaryService = CreateGlossaryService();

      var actual = glossaryService.Translate("Bafana-Bafana", "en-za");
      Assert.AreEqual("Bafana-Bafana", actual);
    }
    
    [Test]
    public void GivenLanguageCode_WhenTranslating_ThenGetTranslated()
    {
      var glossaryService = CreateGlossaryService();

      var actual = glossaryService.Translate("Boiler", "en");
      Assert.AreEqual("Geyser", actual);
    }
    
    [Test]
    public void GivenNoLanguageCode_WhenTranslating_ThenGetInputBack()
    {
      var glossaryService = CreateGlossaryService();

      var actual = glossaryService.Translate("Incorrect Data", null);
      Assert.AreEqual("Incorrect Data", actual);
    }
    
    [Test]
    public void GivenNoInput_WhenTranslating_ThenGetInputBack()
    {
      var glossaryService = CreateGlossaryService();

      var actual = glossaryService.Translate(string.Empty, "Incorrect Data");
      Assert.AreEqual(string.Empty, actual);
    }

    private static IGlossaryService CreateGlossaryService()
    {
      var httpMock = new Mock<IRestApiService>();
      httpMock.Setup(httpClient =>
          httpClient.GetCollection<ApiGlossary>(It.IsAny<RestApiService.GetCollectionParams>()))
        .Returns(new ApiResult<IList<ApiGlossary>>
        {
          Resource = new List<ApiGlossary>
          {
            new ApiGlossary
            {
              Original = "Traffic Light",
              Translated = "Robot",
              LanguageCode = "en-za"
            },
            new ApiGlossary
            {
              Original = "Boiler",
              Translated = "Geyser",
              LanguageCode = "en"
            },
            new ApiGlossary
            {
              Original = "Football",
              Translated = "Soccer",
              LanguageCode = "en"
            }
          }
        });

      var glossaryService = new GlossaryService(
        DataServices.Resolve<IUserService>(),
        DataServices.Resolve<IGlossaryCacheService>(),
        httpMock.Object,
        DataServices.Resolve<ISetupService>(),
        DataServices.Resolve<IWebHookService>(),
        Guid.NewGuid().ToString("N"),
        Guid.NewGuid().ToString("N"));
      
      glossaryService.Start();
      return glossaryService;
    }
  }
}