// //-----------------------------------------------------------------------------
// // <copyright file="BslUserException.cs" company="AMCS Group">
// //   Copyright © 2010-12 AMCS Group. All rights reserved.
// // </copyright>
// //
// // PROJECT: P142 - Elemos
// //
// // AMCS Elemos Project
// //
// //-----------------------------------------------------------------------------

using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  /// <summary>
  ///   User Exception.
  /// </summary>
  [Serializable]
  public class BslUserException : Exception, IBslUserException
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="BslUserException" /> class.
    /// </summary>
    public BslUserException()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="BslUserException" /> class.
    /// </summary>
    /// <param name="errorCode">The error code associated with the error.</param>
    public BslUserException(int errorCode)
    {
      ErrorCode = errorCode;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="BslUserException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BslUserException(string message)
      : base(message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="BslUserException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    public BslUserException(string message, int errorCode)
      : base(message)
    {
      ErrorCode = errorCode;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="BslUserException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a null reference (Nothing in
    ///   Visual Basic) if no inner exception is specified.
    /// </param>
    public BslUserException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="BslUserException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a null reference (Nothing in
    ///   Visual Basic) if no inner exception is specified.
    /// </param>
    public BslUserException(string message, int errorCode, Exception innerException)
      : base(message, innerException)
    {
      ErrorCode = errorCode;
    }

    #region IBslUserException Members

    /// <summary>
    ///   Gets the message prefix.
    /// </summary>
    /// <value>
    ///   The message prefix.
    /// </value>
    public virtual string MessagePrefix
    {
      get { return this.GetType().Name + ":"; }
    }

    /// <summary>
    /// Gets the error code associated with the error.
    /// </summary>
    public int? ErrorCode { get; }

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IList<BslValidationError> Errors { get; set; }

    /// <summary>
    /// Gets the entity object associated with the error message.
    /// </summary>
    public EntityObject EntityObject { get; set; }

    #endregion IBslUserException Members
  }
}