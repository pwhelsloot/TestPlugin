//-----------------------------------------------------------------------------
// <copyright file="IPositionableEntity.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
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
  /// <summary>
  /// Class must adhere to this interface if it is to be positionable in list of its companions.
  /// </summary>
  public interface IPositionableEntity
  {
    int Id32 { get; }
    int? Position { get; set; }
  }
}