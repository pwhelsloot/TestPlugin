using AMCS.Data.Support;
using AMCS.PlatformFramework.IntegrationTests.TestProperties;
using NUnit.Framework;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.Steps
{
  [Binding]
  public class FileFormatHelperSteps
  {
    private const string GIFBASE64 = "GIFBASE64";
    private const string JPGBASE64 = "JPGBASE64";
    private const string PNGBASE64 = "PNGBASE64";
    private byte[] ImageBytesArray=new byte[] {};
    private FileFormat fileFormat;
    private string ExpectedFileFormat=String.Empty;
    private string GivenImageFormat = String.Empty;
    private const string JPEG = "JPEG";
    private const string PNG = "PNG";
    private const string GIF = "GIF";
    private const string NOT_SUPPORTED = "NOTSUPPORTED";
    private string[] ImageHeaderSignature=new string[] {};
    private FileFormatEnum fileFormatEnum;

    [Given(@"an image (.*) image format (.*)")]
    public void GivenAnImageAndImageFormat(string sampleImage, string expectedFileFormat)
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
      ExpectedFileFormat = expectedFileFormat;
    }

    [Given(@"sample image byte array (.*) expected format enum (.*)")]
    public void GivenSampleImageByteArray(string imageHeaderSignature, string expectedFileFormatEnum)
    {
      ImageHeaderSignature= new string[] { imageHeaderSignature };
      ImageHeaderSignature = imageHeaderSignature.Split(',').ToArray();
      switch (expectedFileFormatEnum.ToUpperInvariant())
      {
        case JPEG:
          fileFormatEnum = FileFormatEnum.Jpeg;
          break;
        case GIF:
          fileFormatEnum = FileFormatEnum.Gif;
          break;
        case PNG:
          fileFormatEnum = FileFormatEnum.Png;
          break;
        case NOT_SUPPORTED:
          fileFormatEnum = FileFormatEnum.NotSupported;
          break;
      }
    }

    [When(@"format is fetched with file format validator")]
    public void WhenFormatIsFetchedWithFileFormatValidator()
    {
      if (ImageBytesArray != null)
        fileFormat = FileFormatHelper.GetFileFormat(ImageBytesArray);
      else
      {
        fileFormat = SupportedFileFormats.SupportedList.SingleOrDefault(supportedFileFormat => supportedFileFormat.FileFormatEnum == fileFormatEnum);
        fileFormat = FileFormatHelper.GetFileFormat(ImageHeaderSignature.Select(entry => Convert.ToByte(entry, 16)).ToArray());
      }
    }

    [Then(@"correct format is returned as (.*) expected base64 format")]
    public void ThenCorrectFormatIsReturnedAsExpectedBase64Format(string expectedBase64Format)
    {
      Assert.That(fileFormat.Extension, Is.EqualTo(ExpectedFileFormat));
      Assert.That(fileFormat.Base64Format, Is.EqualTo(expectedBase64Format));
    }

    [Then(@"correct format is returned as (.*) expected format enum")]
    public void ThenCorrectFormatIsReturnedAsJpegExpectedFormatEnum(FileFormatEnum expectedFileFormatEnum)
    {
      Assert.That(fileFormat, Is.EqualTo(fileFormat));
    }
  }
}