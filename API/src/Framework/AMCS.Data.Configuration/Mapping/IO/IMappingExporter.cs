//-----------------------------------------------------------------------------
// <copyright file="IMappingExporter.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.IO
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public interface IMappingExporter
  {
    void Export(IMapping mapping);

    KeyValuePair<IList<IMapping>, IList<Exception>> ExportAll(IList<IMapping> mappings);
  }
}