//-----------------------------------------------------------------------------
// <copyright file="IMappingImporter.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.IO
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml.Linq;

  public interface IMappingImporter
  {
    Assembly XmlAssembly { get; }

    string XmlNamespace { get; }

    IMapping Import(string mappingName);

    IMapping ImportXML(XElement root);

    IDictionary<string, IMapping> ImportAll();
  }
}
