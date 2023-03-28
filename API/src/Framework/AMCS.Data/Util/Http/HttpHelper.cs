namespace AMCS.Data.Util.Http
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Web;

  /// <summary>
  /// Class HttpHelper.
  /// </summary>
  public class HttpHelper
  {
    /// <summary>
    /// Gets the qualified URL (url with parameters) with from raw (base url).
    /// </summary>
    /// <param name="rawURL">The raw URL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>System.String.</returns>
    public static string GetQualifiedURLFromRaw(string rawURL, Dictionary<string, string> parameters)
    {
      var query = HttpUtility.ParseQueryString(string.Empty);
      // Limitation of not having duplicate properties but shouldnt be possible via propetites
      foreach (string key in parameters.Keys)
      {
        query[key] = parameters[key];
      }
      return rawURL + "?" + query.ToString();
    }

    /// <summary>
    /// Converts the objects properties to dictionary
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>Dictionary{System.StringSystem.String}.</returns>
    public static Dictionary<string, string> ConvertObjectsPropertiesToDictionary(object item)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      PropertyInfo[] properties = item.GetType().GetProperties();
      if (properties != null)
      {
        foreach (PropertyInfo prop in properties)
        {
          if (prop.GetGetMethod() != null)
          {
            PropertyInfo fullProp = item.GetType().GetProperty(prop.Name);
            if (fullProp != null && !fullProp.PropertyType.FullName.Contains("ObservableCollection"))
            {
              object fullPropValue = fullProp.GetValue(item, null);
              parameters.Add(prop.Name.ToLower(), fullPropValue != null ? fullPropValue.ToString() : null);
            }
          }
        }
      }
      return parameters;
    }
  }
}
