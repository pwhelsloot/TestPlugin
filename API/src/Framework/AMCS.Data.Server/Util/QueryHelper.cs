namespace AMCS.Data.Server.Util
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Text.Encodings.Web;

  /// <summary>
  /// Note: This is a utility class copied from https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.WebUtilities/QueryHelpers.cs
  /// that lets us easily add parameters to a URL without breaking/losing existing URL parameters. The reason for the copy
  /// is that these helper methods aren't available in .NET Framework.
  /// </summary>
  public static class QueryHelper
  {
    public static string AddQueryString(string uri, string name, string value)
    {
      if (uri == null)
      {
        throw new ArgumentNullException(nameof(uri));
      }

      if (name == null)
      {
        throw new ArgumentNullException(nameof(name));
      }

      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }

      return AddQueryString(
        uri, new[] { new KeyValuePair<string, string>(name, value) });
    }

    public static string AddQueryString(string uri, IDictionary<string, string> queryString)
    {
      if (uri == null)
      {
        throw new ArgumentNullException(nameof(uri));
      }

      if (queryString == null)
      {
        throw new ArgumentNullException(nameof(queryString));
      }

      return AddQueryString(uri, (IEnumerable<KeyValuePair<string, string>>)queryString);
    }

    private static string AddQueryString(
      string uri,
      IEnumerable<KeyValuePair<string, string>> queryString)
    {
      if (uri == null)
      {
        throw new ArgumentNullException(nameof(uri));
      }

      if (queryString == null)
      {
        throw new ArgumentNullException(nameof(queryString));
      }

      var anchorIndex = uri.IndexOf('#');
      var uriToBeAppended = uri;
      var anchorText = "";
      // If there is an anchor, then the query string must be inserted before its first occurence.
      if (anchorIndex != -1)
      {
        anchorText = uri.Substring(anchorIndex);
        uriToBeAppended = uri.Substring(0, anchorIndex);
      }

      var queryIndex = uriToBeAppended.IndexOf('?');
      var hasQuery = queryIndex != -1;

      var sb = new StringBuilder();
      sb.Append(uriToBeAppended);
      foreach (var parameter in queryString)
      {
        sb.Append(hasQuery ? '&' : '?');
        sb.Append(UrlEncoder.Default.Encode(parameter.Key));
        sb.Append('=');
        sb.Append(UrlEncoder.Default.Encode(parameter.Value));
        hasQuery = true;
      }

      sb.Append(anchorText);
      return sb.ToString();
    }
  }
}