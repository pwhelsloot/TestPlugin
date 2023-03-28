namespace AMCS.Data.Util.String
{
  public class StringFormatter
  {
    public static string PascalCaseToTitleCase(string pascalCaseString)
    {
      string result = "";
      foreach (char c in pascalCaseString)
      {
        if (char.IsUpper(c))
          result += " ";
        result += c;
      }
      return result.Trim();
    }
  }
}
