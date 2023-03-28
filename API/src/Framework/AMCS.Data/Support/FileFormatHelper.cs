namespace AMCS.Data.Support
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  // based on https://devblogs.microsoft.com/scripting/psimaging-part-1-test-image/
  public static class FileFormatHelper
  {
    public const int MaxHeaderSize = 12;
    private const string WildcardByte = "*";
    // https://en.wikipedia.org/wiki/List_of_file_signatures
    /* Bytes in c# have a range of 0 to 255 so each byte can be represented as
     * a two digit hex string. */
    private static readonly List<FileFormatMapping> FileFormatSignatures = new List<FileFormatMapping>
    {
      new FileFormatMapping(
       SupportedFileFormats.Jpeg,
        new[]
        {
          new[] {"FF", "D8", "FF", "DB"},
          new[] {"FF", "D8", "FF", "EE"},
          new[] {"FF", "D8", "FF", "E0", "00", "10", "4A", "46", "49", "46", "00", "01"},
          new[] { "FF", "D8", "FF", "E1", WildcardByte, WildcardByte, "45", "78", "69", "66", "00", "00" }
        }),
      new FileFormatMapping(
        SupportedFileFormats.Gif,
        new[]
        {
            new [] { "47", "49", "46", "38", "37", "61" },
            new [] { "47", "49", "46", "38", "39", "61" }
        }),
      new FileFormatMapping(
        SupportedFileFormats.Png,
        new[]
        {
            new[] {"89", "50", "4E", "47", "0D", "0A", "1A", "0A"}
        }),
      new FileFormatMapping(
        SupportedFileFormats.Bmp,
        new []
        {
            new[] { "42", "4D" }
        }),
      new FileFormatMapping(
        SupportedFileFormats.Tiff,
        new []
        {
            new[] { "49", "49", "2A", "00" },
            new[] { "4D", "4D", "00", "2A" },
        }),
      new FileFormatMapping(
        SupportedFileFormats.Emf,
        new []
        {
            new[] { "01", "00", "00", "00" }
        })
    };

    /// <summary>
    /// Takes a byte array and determines the file type by
    /// comparing the first few bytes of the file to a list of known
    /// file signatures.
    /// </summary>
    /// <param name="imageData">Byte array of the file data</param>
    /// <returns>FileFormatMapping corresponding to the file format</returns>
    /// <exception cref="ArgumentException">Thrown if the file type can't be determined</exception>
    private static FileFormatMapping GetFileFormatMapping(byte[] imageData)
    {
      foreach (var fileFormatSignature in FileFormatSignatures)
      {
        foreach (string[] signature in fileFormatSignature.HeaderSignatures)
        {
          bool isMatch = true;
          for (int i = 0; i < signature.Length; i++)
          {
            string signatureByte = signature[i];

            if (signatureByte == WildcardByte)
              continue;

            // ToString("X") gets the hex representation and pads it to always be length 2
            string imageByte = imageData[i]
                .ToString("X2");

            if (signatureByte == imageByte)
              continue;

            isMatch = false;
            break;
          }

          if (isMatch)
          {
            return fileFormatSignature;
          }
        }
      }

      throw new ArgumentException("The byte array did not match any known file signatures.");
    }

    public static FileFormat GetFileFormat(byte[] imageData)
    {
      try
      {
        return GetFileFormatMapping(imageData).FileFormat;
      }
      catch
      {
        return default;
      }
    }

    private struct FileFormatMapping
    {
      public FileFormat FileFormat { get; }

      public string[][] HeaderSignatures { get; }

      public FileFormatMapping(FileFormat fileFormat, string[][] headerSignatures)
      {
        FileFormat = fileFormat;
        HeaderSignatures = headerSignatures;
      }
    }
  }
}
