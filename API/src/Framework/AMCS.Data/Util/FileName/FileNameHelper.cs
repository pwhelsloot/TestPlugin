namespace AMCS.Data.Util.FileName
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;

  public static class FileNameHelper
  {
    private static string[] reservedFileNames = new string[]
    {
	    "con",
	    "prn",
	    "aux",
	    "nul",
	    "com1",
	    "com2",
	    "com3",
	    "com4",
	    "com5",
	    "com6",
	    "com7",
	    "com8",
	    "com9",
	    "lpt1",
	    "lpt2",
	    "lpt3",
	    "lpt4",
	    "lpt5",
	    "lpt6",
	    "lpt7",
	    "lpt8",
	    "lpt9",
	    "clock$"
    };

    public static bool IsFileNameValid(string fileName)
    {
      bool isValid = string.IsNullOrWhiteSpace(fileName);
      if (isValid)
      {
        Regex containsABadCharacter = new Regex("["
            + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
        if (containsABadCharacter.IsMatch(fileName))
        {
          isValid = false;
        };
      }
      if (isValid)
      {
        string toCheck = fileName.Remove(fileName.LastIndexOf(".")).ToLower();
        if (reservedFileNames.Contains(toCheck))
        {
          isValid = false; 
        }
      }

      return isValid;
    }
  }
}
