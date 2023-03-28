using AMCS.Data.Support;
using AMCS.PlatformFramework.IntegrationTests.TestProperties;
using NUnit.Framework;
using System;
using System.IO;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.Steps
{
  [Binding]
  public class FileFormatStreamSteps
  {
    private const string GIFBASE64 = "GIFBASE64";
    private const string JPGBASE64 = "JPGBASE64";
    private const string PNGBASE64 = "PNGBASE64";
    private byte[] ImageBytesArray=new byte[] {};    
    private string ExpectedFileFormat=String.Empty;
    private string GivenImageFormat = String.Empty;
    private string NonFileFormatStreamReaderOutput = String.Empty  ;
    private string FileFormatStreamReaderOutput = String.Empty;
    private Stream Stream;
    private Exception exception=new Exception();
    private MemoryStream MemoryStream=new MemoryStream();
    private FileFormatStream FileFormatStream= null;

    [Given(@"an (.*) image")]
    public void GivenAnImage(string sampleImage)
    {
      switch (sampleImage.ToUpperInvariant())
      {
        case GIFBASE64:
          GivenImageFormat = SampleImages.GifBase64;
          break;
        case PNGBASE64:
          GivenImageFormat = SampleImages.PngBase64;
          break;
        case JPGBASE64:
          GivenImageFormat = SampleImages.JpgBase64;
          break;
      }
      ImageBytesArray = Convert.FromBase64String(GivenImageFormat);
    }

    [When(@"image is read with file stream format validator")]
    public void WhenImageIsReadWithFileStreamFormatValidator()
    {

      using (var ms = new MemoryStream(ImageBytesArray))
      using (var reader = new StreamReader(ms))
      {
        NonFileFormatStreamReaderOutput = reader.ReadToEnd();
      }
      using (var ms = new MemoryStream(ImageBytesArray))
      using (var imageStream = new FileFormatStream(ms))
      using (var reader = new StreamReader(imageStream))
      {
        FileFormatStreamReaderOutput = reader.ReadToEnd();
      }
    }

    [Then(@"correct results are returned")]
    public void ThenCorrectResultsAreReturned()
    {
      Assert.That(FileFormatStreamReaderOutput, Is.EqualTo(NonFileFormatStreamReaderOutput));
    }

    [Given(@"an image (.*) image stream (.*)")]
    public void GivenAnImageAndStream(string sampleImage, string expectedFileExtension)
    {
      GivenAnImage(sampleImage);
      ExpectedFileFormat = expectedFileExtension;
    }

    [When(@"image stream is read using file format validator")]
    public void WhenImageStreamIsReadUsingFileFormatValidator()
    {
      MemoryStream = new MemoryStream(ImageBytesArray);
      FileFormatStream = new FileFormatStream(MemoryStream);
    }

    [Then(@"correct image stream base64 format matches expected result (.*)")]
    public void ThenCorrectImageStreamBaseFormatMatchesExpectedResult(string expectedResult)
    {
      Assert.That(FileFormatStream?.FileFormat.Extension, Is.EqualTo(ExpectedFileFormat));
      Assert.That(FileFormatStream?.FileFormat.Base64Format, Is.EqualTo(expectedResult));
    }

    [When(@"file format stream is called with non zero position")]
    public void WhenFileFormatStreamIsCalledWithNonZeroPosition()
    {
      try
      {
        MemoryStream = new MemoryStream(ImageBytesArray);
        MemoryStream.Position = 1;
        FileFormatStream = new FileFormatStream(MemoryStream);
      }
      catch(Exception ex)
      {
        exception = ex;
      }
    }

    [Given(@"no image")]
    public void GivenNoImage()
    {
      Stream = null;
    }

    [When(@"file format stream is called")]
    public void WhenFileFormatStreamIsCalled()
    {
      try
      {
        var imageStream = new FileFormatStream(Stream);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    [Then(@"exception is thrown")]
    public void ThenExceptionIsThrown()
    {
      if(exception.GetType()== typeof(ArgumentNullException))
      Assert.AreEqual(typeof(ArgumentNullException), exception.GetType());
      else if (exception.GetType() == typeof(InvalidOperationException))
        Assert.AreEqual(typeof(InvalidOperationException), exception.GetType());
    }
  }
}