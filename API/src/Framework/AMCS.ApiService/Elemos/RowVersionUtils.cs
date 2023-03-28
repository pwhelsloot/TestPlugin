using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;
using AMCS.Encryption;

namespace AMCS.ApiService.Elemos
{
  internal static class RowVersionUtils
  {
    private static readonly byte[] EmptyRowVersion = new byte[0];

    private const string RowVersionEncryptionKey = "k2IXynGZV6IctCDQpvBepVG9QuHGBEIojGt2fft0GMNVhnXNu6WrAKZa97m16SI";
    private static readonly IEncryptionService RowVersionEncryptionService = EncryptionServiceFactory.Create(RowVersionEncryptionKey);

    public static byte[] ParseRowVersion(string rowVersion)
    {
      return string.IsNullOrEmpty(rowVersion)
        ? EmptyRowVersion
        : Escaping.HexDecode(rowVersion);
    }

    public static byte[] ParseEncryptedRowVersion(string rowVersion)
    {
      return string.IsNullOrEmpty(rowVersion)
        ? EmptyRowVersion
        : ParseRowVersion(RowVersionEncryptionService.Decrypt(rowVersion));
    }

    public static string PrintRowVersion(byte[] rowVersion)
    {
      return rowVersion == null || rowVersion.Length == 0
        ? string.Empty
        : Escaping.HexEncode(rowVersion);
    }

    public static string PrintEncryptedRowVersion(byte[] rowVersion)
    {
      return rowVersion == null || rowVersion.Length == 0
        ? string.Empty
        : RowVersionEncryptionService.Encrypt(PrintRowVersion(rowVersion));
    }
  }
}
