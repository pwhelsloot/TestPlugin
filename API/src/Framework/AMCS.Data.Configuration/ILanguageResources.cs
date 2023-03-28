//-----------------------------------------------------------------------------
// <copyright file="ILanguageResources.cs" company="AMCS Group">
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

  public interface ILanguageResources
  {
    Assembly Assembly { get; }

    string StringResourcesNamespace { get; }

    string EntityResourcesNamespace { get; }
    
    string SearchResourcesNamespace { get; }
  }
}
