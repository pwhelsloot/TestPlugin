//-----------------------------------------------------------------------------
// <copyright file="ValidationEventArgs.cs" company="AMCS Group">
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

namespace AMCS.Data.Entity
{
  public delegate void ValidationEventHandler(object sender, ValidationEventArgs e);

  public class ValidationEventArgs : EventArgs
  {
    public EntityObject Entity;
    public string PropertyName;

    public bool Handled;
    public string Error;

    public ValidationEventArgs()
      : base()
    {
    }
  }
}