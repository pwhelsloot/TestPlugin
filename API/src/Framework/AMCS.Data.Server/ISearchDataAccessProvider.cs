//-----------------------------------------------------------------------------
// <copyright file="ISearchDataAccessProvider.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.Search;

namespace AMCS.Data.Server
{
  public interface ISearchDataAccessProvider
  {
    SearchResultsEntity GetSearchResultsFromReader(string searchResultsId, IDataReader rdr);
  }
}