//-----------------------------------------------------------------------------
// <copyright file="ITranslatableAggregate.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public interface ITranslatableAggregate
  {
    string Type { get; set; }

    string StringFormat { get; set; }

    ITranslatableLocaleString Caption { get; set; }
  }
}
