//-----------------------------------------------------------------------------
// <copyright file="BslUserAccessDeniedException.cs" company="AMCS Group">
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
  /// User Access Denied Exception.
  /// </summary>
  public class BslUserAccessDeniedException : BslUserException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BslUserAccessDeniedException"/> class.
    /// </summary>
    public BslUserAccessDeniedException()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BslUserAccessDeniedException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public BslUserAccessDeniedException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BslUserAccessDeniedException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public BslUserAccessDeniedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}