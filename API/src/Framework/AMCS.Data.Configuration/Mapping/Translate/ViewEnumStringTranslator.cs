//-----------------------------------------------------------------------------
// <copyright file="ViewEnumStringTranslator.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.Translate
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class ViewEnumStringTranslator : EnumeratedStringCollectionTranslator
  {
    public ViewEnumStringTranslator(string viewFullName, Type stringsEnumType) :
      base(ResourceStringOwnerType.View, viewFullName, stringsEnumType)
    {
    }
  }
}
