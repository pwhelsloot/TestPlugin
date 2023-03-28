namespace AMCS.Data.Support
{
  using System.IO;
  using System.Security.Cryptography;
  using System.Text;

  public static class StringEncryption
  {
    public static string Sha1Encode(string data)
    {
      return Sha1Encode(Encoding.UTF8.GetBytes(data));
    }

    public static string Sha1Encode(byte[] data)
    {
      byte[] hash;

      using (var source = new MemoryStream(data))
      using (var sha1 = new SHA1Managed())
      {
        hash = sha1.ComputeHash(source);
      }

      return Escaping.HexEncode(hash);
    }
  }
}