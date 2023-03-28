﻿//-----------------------------------------------------------------------------
// <copyright file="ISearchResultMapping.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public interface ISearchResultMapping : IMapping, IAuditMapping
  {
    IList<ISearchResultColumn> Columns { get; set; }
  }
}
