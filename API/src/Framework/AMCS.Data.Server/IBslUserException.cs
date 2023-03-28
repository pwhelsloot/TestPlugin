//-----------------------------------------------------------------------------
// <copyright file="IBslUserException.cs" company="AMCS Group">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  /// <summary>
  /// User Exception Interface.
  /// </summary>
  public interface IBslUserException
  {
    /// <summary>
    /// Gets the message prefix.
    /// </summary>
    /// <value>
    /// The message prefix.
    /// </value>
    string MessagePrefix { get; }
  }
}