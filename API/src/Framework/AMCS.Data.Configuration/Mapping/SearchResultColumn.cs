//-----------------------------------------------------------------------------
// <copyright file="SearchResultColumn.cs" company="AMCS Group">
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

  public class SearchResultColumn : ISearchResultColumn
  {
    public string ColumnName { get; set; }

    public bool IsKey { get; set; }

    public int? Width { get; set; }

    public string StringFormat { get; set; }

    public int? DisplayType { get; set; }

    public string ImageReferenceOne { get; set; }

    public string ImageReferenceTwo { get; set; }

    public int? ImageHeight { get; set; }

    public int? ImageWidth { get; set; }

    public bool Editable { get; set; }

    public ITranslatableLocaleString String { get; set; }

    public ITranslatableAggregate[] AggregateFunctions { get; set; }
  }
}
