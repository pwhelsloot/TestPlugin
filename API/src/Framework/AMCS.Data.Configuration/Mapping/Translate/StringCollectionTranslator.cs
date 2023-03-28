//-----------------------------------------------------------------------------
// <copyright file="StringCollectionTranslator.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.Translate
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration.Resource;

  public class StringCollectionTranslator : Translator
  {
    private ResourceStringOwnerType ownerType;
    private string ownerName;
    private ILocalisedStringResourceCache stringCache;

    public StringCollectionTranslator(ResourceStringOwnerType ownerType, string ownerName) : base()
    {
      this.ownerType = ownerType;
      this.ownerName = ownerName;
      this.stringCache = DataServices.Resolve<ILocalisedStringResourceCache>();
    }

    public virtual string GetLocalisedString(string defaultValue)
    {
      string primaryKey = string.Format("{0}#{1}:{2}", this.ownerType, this.ownerName, defaultValue);
      string result = this.stringCache.GetString(primaryKey, defaultValue);
      if (string.IsNullOrWhiteSpace(result))
      {
        result = defaultValue;
      }

      return result;
    }
  }
}
