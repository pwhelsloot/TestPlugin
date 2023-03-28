//-----------------------------------------------------------------------------
// <copyright file="DataOperationIgnoreInheritedPropertiesAttribute.cs" company="AMCS Group">
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

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public class DataOperationIgnoreInheritedProperties : Attribute
  {
    [Flags]
    public enum DataOperationType { Read = 1, Write = 2, All = 3 }

    public string[] Except { get; private set; }
    public DataOperationType IgnoreForDataOperationType { get; private set; }

    public DataOperationIgnoreInheritedProperties()
    {
      IgnoreForDataOperationType = DataOperationType.All;
      Except = new string[0];
    }

    public DataOperationIgnoreInheritedProperties(DataOperationType ignoreForDataOperationType)
    {
      IgnoreForDataOperationType = ignoreForDataOperationType;
      Except = new string[0];
    }

    public DataOperationIgnoreInheritedProperties(string[] except)
    {
      IgnoreForDataOperationType = DataOperationType.All;
      Except = except;
    }

    public DataOperationIgnoreInheritedProperties(DataOperationType ignoreForDataOperationType, string[] except)
    {
      IgnoreForDataOperationType = ignoreForDataOperationType;
      Except = except;
    }
  }
}