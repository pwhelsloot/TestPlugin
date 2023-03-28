namespace AMCS.Data.SourceGenerator.ExtensionMethods
{
  internal static class StringExtensionMethods
  {
    public static string FirstCharToLowerCase(this string text) => text.Substring(0, 1).ToLowerInvariant() + text.Substring(1);
  }
}
