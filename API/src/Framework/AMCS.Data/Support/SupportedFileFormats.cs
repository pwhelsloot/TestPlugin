namespace AMCS.Data.Support
{
  using System.Collections.Generic;

  public static class SupportedFileFormats
  {
    public static List<FileFormat> SupportedList => new List<FileFormat> { Jpeg, Gif, Png, Bmp };
    public static FileFormat Jpeg = new FileFormat(FileFormatEnum.Jpeg, ".jpeg", "data:image/jpeg;base64,", "image/jpeg");
    public static FileFormat Gif = new FileFormat(FileFormatEnum.Gif, ".gif", "data:image/gif;base64,", "image/gif");
    public static FileFormat Png = new FileFormat(FileFormatEnum.Png, ".png", "data:image/png;base64,", "image/png");
    public static FileFormat Bmp = new FileFormat(FileFormatEnum.Bmp, ".bmp", "data:image/bmp;base64,", "image/bmp");
    public static FileFormat Tiff = new FileFormat(FileFormatEnum.Tiff, ".tif", "data:image/tiff;base64,", "image/tiff");
    public static FileFormat Emf = new FileFormat(FileFormatEnum.Emf, ".emf", "data:image/emf;base64,", "image/emf");
  }
}
