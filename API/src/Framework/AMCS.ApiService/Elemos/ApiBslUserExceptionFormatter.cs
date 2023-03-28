using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos
{
  public static class ApiBslUserExceptionFormatter
  {
    public const string ErrorStart = "/***";
    public const string ErrorEnd = "***/";
    public const string NewLine = "***/n***";

    public static string FormatError(string message)
    {
      StringBuilder stringBuilder = new StringBuilder(ErrorStart);
      stringBuilder.Append(message.Replace(Environment.NewLine, NewLine));
      stringBuilder.Append(ErrorEnd);
      return stringBuilder.ToString();
    }
  }
}
