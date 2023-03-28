//-----------------------------------------------------------------------------
// <copyright file="IMapping.cs" company="AMCS Group">
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

  public interface IMapping
  {
    string Id { get; set; }

    Guid FileId { get; set; }

    /// <summary>
    /// Implementation should return true if the structure of some mapping matches another.
    /// It must not take into account any non-structural ascpects of a mapping, such as translations or other displayable 
    /// characteristics
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool MappedStructureEquals(IMapping other);

    /// <summary>
    /// Merges a mapping into "this" one.  It is possible for "mergeIn" to be a partial mapping 
    /// so nothing should be removed from "this".
    /// The rules that must be followed in an implementation are:
    /// If there are any changes between fields on "this" and "mergeIn" then the field in "mergeIn" will be taken.
    /// If there are any fields in "mergeIn" that do not exist in "this" then add them to "this".
    /// If there are any fields on "this" that do not exist in "mergeIn" then do nothing - keep the field on "this".
    /// </summary>
    /// <param name="mergeIn"></param>
    void MergeWith(IMapping mergeIn);
  }
}