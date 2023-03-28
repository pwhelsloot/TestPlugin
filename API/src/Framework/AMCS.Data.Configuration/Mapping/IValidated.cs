//-----------------------------------------------------------------------------
// <copyright file="IValidated.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
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

  public interface IValidated
  {
    string FailValidationTest { get; set; }
  }
}
