using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions
{
  public static class ApiResourceUtils
  {
    public static string FriendlyId(Type type, bool fullyQualified = false)
    {
      string result;
      if (fullyQualified)
        result = FullNameSansTypeParameters(type).Replace("+", ".");
      else
        result = type.Name;

      if (!type.IsGenericType)
        return result;

      string[] genericArguments = type.GetGenericArguments().Select(p => FriendlyId(p, fullyQualified)).ToArray();

      result = result.Replace($"`{genericArguments.Length}", string.Empty);

      return result + $"[{string.Join(",", genericArguments).TrimEnd(',') as object}]";
    }

    public static string FullNameSansTypeParameters(Type type)
    {
      string result = type.FullName;
      if (string.IsNullOrEmpty(result))
        result = type.Name;

      int length = result.IndexOf("[[", StringComparison.Ordinal);
      if (length != -1)
        return result.Substring(0, length);

      return result;
    }
  }
}
