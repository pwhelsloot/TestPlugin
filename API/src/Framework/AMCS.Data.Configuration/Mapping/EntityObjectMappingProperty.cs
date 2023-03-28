//-----------------------------------------------------------------------------
// <copyright file="EntityObjectMappingProperty.cs" company="AMCS Group">
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

  public class EntityObjectMappingProperty : IEntityObjectProperty
  {
    public string PropertyName { get; set; }

    public string ColumnName { get; set; }

    public bool IsKey { get; set; }

    public bool IsDynamic { get; set; }

    public string StringFormat { get; set; }

    public int? Width { get; set; }

    public string FailValidationTest { get; set; }

    public ITranslatableLocaleString String { get; set; }
  }
}
