using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AMCS.Data.Configuration.Mapping
{
  public static class EmbeddedResourceHelper
  {
    public static List<(string ProjectIdentifier, string FileName)> GetEmbeddedResourcePaths(Assembly assembly, string @namespace, string fileExtension)
    {
      var embeddedResourcePaths = new List<(string projectIdentifier, string fileName)>();
      var embeddedResourceNames = Array.FindAll(assembly.GetManifestResourceNames(), x => x.Contains(@namespace) && x.EndsWith(fileExtension));
      if (embeddedResourceNames.Length > 0)
      {
        foreach (string embeddedResourceName in embeddedResourceNames)
        {
          string projectIdentifier = string.Empty;
          string fileName;
          string toRemove = $"{@namespace}.";
          if (embeddedResourceName.StartsWith(toRemove))
          {
            fileName = Path.GetFileNameWithoutExtension(embeddedResourceName.Substring(embeddedResourceName.IndexOf(toRemove, StringComparison.OrdinalIgnoreCase) + toRemove.Length));

            int indexOfDot = fileName.IndexOf(".", StringComparison.OrdinalIgnoreCase);
            if (indexOfDot > -1)
            {
              projectIdentifier = fileName.Substring(0, indexOfDot);
            }
          }
          else
          {
            fileName = Path.GetFileNameWithoutExtension(embeddedResourceName);

            int indexOfDot = fileName.IndexOf(".", StringComparison.OrdinalIgnoreCase);
            if (indexOfDot > -1)
            {
              projectIdentifier = fileName.Substring(0, indexOfDot);
            }
          }

          embeddedResourcePaths.Add((projectIdentifier, embeddedResourceName));
        }
      }

      return embeddedResourcePaths;
    }
  }
}