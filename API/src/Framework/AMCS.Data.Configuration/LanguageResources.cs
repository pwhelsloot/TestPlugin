//-----------------------------------------------------------------------------
// <copyright file="LanguageResources.cs" company="AMCS Group">
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
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;

  public abstract class LanguageResources : ILanguageResources
  {
    public Assembly Assembly => GetType().Assembly;

    public string StringResourcesNamespace { get; }

    public string EntityResourcesNamespace { get; }

    public string SearchResourcesNamespace { get; }

    protected LanguageResources()
      : this("ent", "srch")
    {
    }

    protected LanguageResources(string entityResourcesPath, string searchResourcesPath)
    {
      this.StringResourcesNamespace = GetType().Namespace;
      
      this.EntityResourcesNamespace = this.StringResourcesNamespace + "." + entityResourcesPath;
      
      this.SearchResourcesNamespace = this.StringResourcesNamespace + "." + searchResourcesPath;
    }
  }
}