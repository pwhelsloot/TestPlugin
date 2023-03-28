namespace AMCS.Data.Support
{
  using System.Collections.Generic;

  public struct FileFormat
  {
    public FileFormatEnum FileFormatEnum { get; }
    public string Extension { get; }
    public string Base64Format { get; }
    public string MIMEType { get; }

    public FileFormat(FileFormatEnum fileFormat, string extension, string base64Format, string mimeType)
    {
      FileFormatEnum = fileFormat;
      Extension = extension;
      Base64Format = base64Format;
      MIMEType = mimeType;
    }
  }
}
