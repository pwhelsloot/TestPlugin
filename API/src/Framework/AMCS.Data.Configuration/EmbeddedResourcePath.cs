//-----------------------------------------------------------------------------
// <copyright file="EmbeddedResourcePath.cs" company="AMCS Group">
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
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class EmbeddedResourcePath
  {
    public string ProjectIdentifier { get; }

    public string FileName { get; }

    public EmbeddedResourcePath(string projectIdentifier, string fileName)
    {
      this.ProjectIdentifier = projectIdentifier;
      this.FileName = fileName;
    }
  }
}
