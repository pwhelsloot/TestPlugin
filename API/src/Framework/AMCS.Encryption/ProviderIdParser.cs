using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AMCS.Encryption
{
  /// <remarks>
  /// We prefix the encrypted data (before the base64 encode) with a prefix to indicate
  /// the encryption provider. The prefix pattern chosen for this is {-1-} for provider
  /// ID 1. The reason for the format of this prefix is to limit the risk it matches
  /// something encoded using the legacy encryptor. A quick test of a pattern similar
  /// to {1} showed 0.00006% chance of conflicts, which is low but not zero. With the
  /// more complex prefix, we didn't find any conflicts in over 500,000,000 random encryptions.
  /// </remarks>
  internal static class ProviderIdParser
  {
    public static byte[] Print(int providerId)
    {
      return Encoding.ASCII.GetBytes("{-" + providerId.ToString(CultureInfo.InvariantCulture) + "-}");
    }

    public static int? Parse(ref byte[] data)
    {
      if (data.Length <= 4)
        return null;

      // Check for the {- prefix.

      if (data[0] != '{' || data[1] != '-')
        return null;

      // Find the number.

      const int start = 2;
      int offset = start;

      while (offset < data.Length)
      {
        byte b = data[offset];
        if (b < '0' || b > '9')
          break;
        offset++;
      }

      // Check for the -} postfix.

      if (offset == start || offset > data.Length - 2)
        return null;

      if (data[offset] != '-' || data[offset + 1] != '}')
        return null;

      // Ensure we have at least something that would represent an integer.

      if (!int.TryParse(Encoding.ASCII.GetString(data, start, offset - start), NumberStyles.None, CultureInfo.InvariantCulture, out int providerId))
        return null;

      // Remove the provider ID from the input array.

      int end = offset + 2;

      byte[] result = new byte[data.Length - end];

      Array.Copy(data, end, result, 0, result.Length);

      data = result;

      return providerId;
    }
  }
}
