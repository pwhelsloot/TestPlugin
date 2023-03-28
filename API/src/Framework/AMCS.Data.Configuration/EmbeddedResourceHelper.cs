//-----------------------------------------------------------------------------
// <copyright file="EmbeddedResourceHelper.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;

  public static class EmbeddedResourceHelper
  {
    public static List<EmbeddedResourcePath> GetEmbeddedResourcePaths(Assembly assembly, string namespaceName, string fileExtension)
    {
      var embeddedResourcePaths = new List<EmbeddedResourcePath>();

      var manifestResourceNames = assembly.GetManifestResourceNames();
      var embeddedResourceNames = Array.FindAll(manifestResourceNames, x => x.Contains(namespaceName) && x.EndsWith(fileExtension));

      if (embeddedResourceNames.Length > 0)
      {
        foreach (string embeddedResourceName in embeddedResourceNames)
        {
          string projectIdentifier = string.Empty;
          string fileName;
          string toRemove = $"{namespaceName}.";
          bool startsWith = embeddedResourceName.StartsWith(toRemove);

          if (startsWith)
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

          embeddedResourcePaths.Add(new EmbeddedResourcePath(projectIdentifier, embeddedResourceName));
        }
      }

      return embeddedResourcePaths;
    }
  }
}