//-----------------------------------------------------------------------------
// <copyright file="ILocalisedStringResourceCache.cs" company="AMCS Group">
//   Copyright © 2010-16 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace AMCS.Data.Configuration.Resource
{
  public interface ILocalisedStringResourceCache
  {
    string GetString(string primaryKey, string fallbackKey);

    List<(string, string)> GetStrings(string primaryKey);

    List<string> GetLocales();
  }
}
