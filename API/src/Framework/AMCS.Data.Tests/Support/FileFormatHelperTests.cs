namespace AMCS.Data.Tests.Support
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Support;
  using NUnit.Framework;

  public class ImageFormatHelperTests
  {
    [TestCase(SampleImages.JpgBase64, ".jpeg", "data:image/jpeg;base64,")]
    [TestCase(SampleImages.GifBase64, ".gif", "data:image/gif;base64,")]
    [TestCase(SampleImages.PngBase64, ".png", "data:image/png;base64,")]
    public void GetImageFormat_Returns_Correct_Results(string sampleImage, string expectedFileFormat, string expectedBase64Format)
    {
      var sampleImageByteArray = Convert.FromBase64String(sampleImage);
      var fileFormat = FileFormatHelper.GetFileFormat(sampleImageByteArray);
      Assert.That(fileFormat.Extension, Is.EqualTo(expectedFileFormat));
      Assert.That(fileFormat.Base64Format, Is.EqualTo(expectedBase64Format));
    }

    [TestCase(new[] { "FF", "D8", "FF", "DB" }, FileFormatEnum.Jpeg)]
    [TestCase(new[] { "FF", "D8", "FF", "EE" }, FileFormatEnum.Jpeg)]
    [TestCase(new[] { "FF", "D8", "FF", "E0", "00", "10", "4A", "46", "49", "46", "00", "01" }, FileFormatEnum.Jpeg)]
    [TestCase(new[] { "FF", "D8", "FF", "E1", "AA", "AA", "45", "78", "69", "66", "00", "00" }, FileFormatEnum.Jpeg)]
    [TestCase(new[] { "FF", "D8", "FF", "E1", "FF", "FF", "45", "78", "69", "66", "00", "00" }, FileFormatEnum.Jpeg)]
    [TestCase(new[] { "47", "49", "46", "38", "37", "61" }, FileFormatEnum.Gif)]
    [TestCase(new[] { "47", "49", "46", "38", "39", "61" }, FileFormatEnum.Gif)]
    [TestCase(new[] { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" }, FileFormatEnum.Png)]
    [TestCase(new[] { "42", "4D" }, FileFormatEnum.Bmp)]
    [TestCase(new[] { "FF", "D8", "DB", "DB" }, FileFormatEnum.NotSupported)]
    [TestCase(new[] { "FF", "D8", "FF", "E1" }, FileFormatEnum.NotSupported)]
    public void GetImageFormat_FromSampleHeaderBytes_Returns_Correct_Results(string[] imageHeaderSignature, FileFormatEnum expectedFileFormatEnum)
    {
      var expectedFileFormat = SupportedFileFormats.SupportedList.SingleOrDefault(supportedFileFormat => supportedFileFormat.FileFormatEnum == expectedFileFormatEnum);
      var fileFormat = FileFormatHelper.GetFileFormat(imageHeaderSignature.Select(entry => Convert.ToByte(entry, 16)).ToArray());
      Assert.That(fileFormat, Is.EqualTo(expectedFileFormat));
    }
  }
}