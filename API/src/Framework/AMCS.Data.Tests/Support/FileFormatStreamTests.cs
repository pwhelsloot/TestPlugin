namespace AMCS.Data.Tests.Support
{
  using System;
  using System.IO;
  using AMCS.Data.Support;
  using NUnit.Framework;

  public class FileFormatStreamTests
  {
    [TestCase(SampleImages.JpgBase64, ".jpeg", "data:image/jpeg;base64,")]
    [TestCase(SampleImages.GifBase64, ".gif", "data:image/gif;base64,")]
    [TestCase(SampleImages.PngBase64, ".png", "data:image/png;base64,")]
    public void FileExtension_Returns_Correct_Value(string sampleImage, string expectedFileExtension, string expectedBase64Format)
    {
      using (var stream = new MemoryStream(Convert.FromBase64String(sampleImage)))
      using (var imageStream = new FileFormatStream(stream))
      {
        Assert.That(imageStream.FileFormat.Extension, Is.EqualTo(expectedFileExtension));
        Assert.That(imageStream.FileFormat.Base64Format, Is.EqualTo(expectedBase64Format));
      }
    }

    [TestCase(SampleImages.JpgBase64)]
    [TestCase(SampleImages.GifBase64)]
    [TestCase(SampleImages.PngBase64)]
    public void Read_Returns_Correct_Value(string sampleImage)
    {
      string fsResult;
      using (var ms = new MemoryStream(Convert.FromBase64String(sampleImage)))
      using (var reader = new StreamReader(ms))
      {
        fsResult = reader.ReadToEnd();
      }

      string isResult;

      using (var ms = new MemoryStream(Convert.FromBase64String(sampleImage)))
      using (var imageStream = new FileFormatStream(ms))
      using (var reader = new StreamReader(imageStream))
      {
        isResult = reader.ReadToEnd();
      }
      Assert.That(isResult, Is.EqualTo(fsResult));
    }

    [Test]
    public void Constructor_With_Null_Throws_NullException()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        using (var imageStream = new FileFormatStream(null)) { };
      });
    }

    [Test]
    public void Constructor_With_Stream_With_NonZero_Position_Throws_Exception()
    {
      Assert.Throws<InvalidOperationException>(() =>
      {
        using (var ms = new MemoryStream(Convert.FromBase64String(SampleImages.JpgBase64)))
        {
          ms.Position = 1;
          using (var imageStream = new FileFormatStream(ms)) { };
        };
      });
    }
  }
}