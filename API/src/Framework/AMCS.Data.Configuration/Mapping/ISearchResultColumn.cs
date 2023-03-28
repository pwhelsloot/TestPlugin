//-----------------------------------------------------------------------------
// <copyright file="ISearchResultColumn.cs" company="AMCS Group">
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

  public interface ISearchResultColumn : IColumn, ISizeable
  {
    string StringFormat { get; set; }

    int? DisplayType { get; set; }

    string ImageReferenceOne { get; set; }

    string ImageReferenceTwo { get; set; }

    int? ImageHeight { get; set; }

    int? ImageWidth { get; set; }

    bool Editable { get; set; }

    ITranslatableLocaleString String { get; set; }

    ITranslatableAggregate[] AggregateFunctions { get; set; }
  }
}
