//-----------------------------------------------------------------------------
// <copyright file="StringCollectionMapping.cs" company="AMCS Group">
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

  public class StringCollectionMapping : IStringCollectionMapping
  {
    public string Id { get; set; }

    public Guid FileId { get; set; }

    public IList<ITranslatableLocaleString> TranslatableStrings { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool MappedStructureEquals(IMapping other)
    {
      StringCollectionMapping that = (StringCollectionMapping)other;
      if (!this.Id.Equals(that.Id))
      {
        return false;
      }

      int thisStringCount = 0;
      int thatStringCount = 0;
      if (this.TranslatableStrings != null)
      {
        thisStringCount = this.TranslatableStrings.Count;
      }
        
      if (that.TranslatableStrings != null)
      {
        thatStringCount = that.TranslatableStrings.Count;
      }

      if (thisStringCount != thatStringCount)
      {
        return false;
      }

      foreach (ITranslatableLocaleString tranString in this.TranslatableStrings)
      {
        // We have the same number of strings, if there are to be any differences then at 
        // least one will have to have a different name
        if (that.TranslatableStrings.FirstOrDefault(s => s.Value.Equals(tranString.Value)) == null)
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// See comments in IMapping.
    /// </summary>
    /// <param name="mergeIn"></param>
    public void MergeWith(IMapping mergeIn)
    {
      if (mergeIn is StringCollectionMapping)
      {
        foreach (ITranslatableLocaleString mergeInTranLocString in ((StringCollectionMapping)mergeIn).TranslatableStrings)
        {
          if (mergeInTranLocString.Translations != null && mergeInTranLocString.Translations.Count > 0)
          {
            ITranslatableLocaleString tranLocString = this.TranslatableStrings.FirstOrDefault(tls => tls.Value.Equals(mergeInTranLocString.Value));
            if (tranLocString != default(ITranslatableLocaleString))
            {
              foreach (ILocaleString mergeInlocString in mergeInTranLocString.Translations)
              {
                ILocaleString match = tranLocString.Translations.FirstOrDefault(t => t.Locale == mergeInlocString.Locale);
                if (match != default(ILocaleString))
                {
                  match.Value = mergeInlocString.Value;
                }
                else
                {
                  tranLocString.Translations.Add(mergeInlocString);
                }
              }
            }
            else
            {
              // The translation wasn't found so add it
              this.TranslatableStrings.Add(mergeInTranLocString);
            }
          }
        }
      }
    }
  }
}
