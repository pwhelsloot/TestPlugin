using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Util
{
  public static class CultureInfoUtils
  {
    /// <summary>
    /// Check whether a culture name is defined.
    /// </summary>
    /// <remarks>
    /// In older versions of Windows, <see cref="CultureInfo.GetCultureInfo(string)"/> would throw
    /// an exception on undefined cultures. Newer versions however will create a culture regardless
    /// of whether it's a proper one.
    ///
    /// This method iterates over all cultures defined by the system and checks whether there's one
    /// that matches the provided name.
    /// </remarks>
    /// <param name="name">The culture info name.</param>
    /// <returns>Whether a culture exists of the specified name.</returns>
    public static bool IsCultureInfoDefined(string name)
    {
      foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
      {
        if (string.Equals(cultureInfo.Name, name, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }
  }
}
