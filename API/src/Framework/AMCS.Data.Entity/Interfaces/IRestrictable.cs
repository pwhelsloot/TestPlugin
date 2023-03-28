//-----------------------------------------------------------------------------
// <copyright file="IRestrictableEntity.cs" company="AMCS Group">
//   Copyright © 2013 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Interfaces
{
  public interface IRestrictable
  {
    string Description { get; }
  }
}