//-----------------------------------------------------------------------------
// <copyright file="XmlMappingImporter.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.IO.Xml
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml.Linq;

  public abstract class XmlMappingImporter : IMappingImporter
  {
    public static readonly XNamespace XmlNs = "http://www.solutionworks.co.uk";

    public const string XmlFileExtension = ".xml";

    public Assembly XmlAssembly { get; }

    public string XmlNamespace { get; }

    public XmlMappingImporter(Assembly xmlAssembly, string xmlNamespace)
    {
      if (xmlAssembly == null || string.IsNullOrEmpty(xmlNamespace))
      {
        throw new Exception("Mapping assembly or namespace is missing.");
      }

      this.XmlAssembly = xmlAssembly;
      this.XmlNamespace = xmlNamespace;
    }

    #region Abstract

    public abstract IMapping Import(string filename);

    public abstract IMapping ImportXML(XElement root);

    #endregion Abstract

    #region Methods

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual IDictionary<string, IMapping> ImportAll()
    {
      IDictionary<string, IMapping> mappings = new Dictionary<string, IMapping>();

      var embeddedResources = EmbeddedResourceHelper.GetEmbeddedResourcePaths(this.XmlAssembly, this.XmlNamespace, XmlFileExtension);

      foreach (var fileName in embeddedResources.Where(e => e.ProjectIdentifier == string.Empty).Select(e => e.FileName))
      {
        IMapping mapping = this.Import(fileName);

        if (mappings.ContainsKey(mapping.Id))
        {
          throw new Exception($"Problem with {fileName}, mapping {mapping.Id} already exists");
        }

        mappings.Add(mapping.Id, mapping);
      }

      return mappings;
    }

    #endregion Methods
  }
}