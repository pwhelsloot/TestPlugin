using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger.Documentation
{
  internal class MarkdownDocumentationManager
  {
    private readonly Assembly assembly;
    private readonly Dictionary<string, string> resourceNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public MarkdownDocumentationManager(Type location)
    {
      assembly = location.Assembly;

      string prefix = location.Namespace + ".";

      foreach (string resourceName in assembly.GetManifestResourceNames())
      {
        if (resourceName.StartsWith(prefix))
        {
          resourceNames.Add(
            resourceName.Substring(prefix.Length, resourceName.Length - prefix.Length),
            resourceName);
        }
      }
    }

    public string GetDocumentation(string relativePath, string method)
    {
      if (!TryGetAnyResourceName(relativePath, ".md", method, out string resourceName))
        return null;

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      using (var reader = new StreamReader(stream))
      {
        // We don't have to format Markdown, because Swagger UI renders
        // this content using a JavaScript Markdown renderer.

        return reader.ReadToEnd();
      }
    }

    public string GetDocument(string path)
    {
      int pos = path.LastIndexOf('.');

      string extension = path.Substring(pos);
      path = path.Substring(0, pos);

      return GetDocument(path, extension);
    }

    public string GetDocument(string path, string extension)
    {
      string relativePath = path.TrimStart('/').Replace('/', '.');

      if (!TryGetResourceName(relativePath, extension, out string resourceName))
        return null;

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      using (var reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }

    private bool TryGetAnyResourceName(string path, string extension, string method, out string resourceName)
    {
      if (
        TryGetResourceName(path + "." + method, extension, out resourceName) ||
        TryGetResourceName(path, extension, out resourceName))
        return true;

      int index = path.LastIndexOf('.');
      if (index != -1)
        return TryGetAnyResourceName(path.Substring(0, index), extension, method, out resourceName);

      return false;
    }

    private bool TryGetResourceName(string path, string extension, out string resourceName)
    {
      return resourceNames.TryGetValue(path + extension, out resourceName);
    }
  }
}
