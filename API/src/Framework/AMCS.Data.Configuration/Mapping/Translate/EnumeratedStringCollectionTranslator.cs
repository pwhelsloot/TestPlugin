//-----------------------------------------------------------------------------
// <copyright file="EnumeratedStringCollectionTranslator.cs" company="AMCS Group">
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
  using System.Collections.ObjectModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Reflection;
  using Newtonsoft.Json;
  using Util.Extension;

  [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
  public abstract class EnumeratedStringCollectionTranslator : StringCollectionTranslator
  {
    private readonly string ownerTypeFullName;
    private readonly Type stringsEnumType;
    private readonly Dictionary<string, int> byLocalisedString = new Dictionary<string, int>();
    private readonly Dictionary<int, string> byValue = new Dictionary<int, string>();
    private readonly Dictionary<int, string> defaultStringsByValue = new Dictionary<int, string>();
    private readonly IList<string> localisedStrings;

    public EnumeratedStringCollectionTranslator(ResourceStringOwnerType ownerType, string ownerTypeFullName, Type stringsEnumType) :
      base(ownerType, ownerTypeFullName)
    {
      this.ownerTypeFullName = ownerTypeFullName;
      this.stringsEnumType = stringsEnumType;

      if (this.stringsEnumType == null)
      {
        throw new Exception($"Internal Error: The string enumeration type hasn't been defined for {this.ownerTypeFullName}");
      }

      var fields = this.stringsEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
      if (fields == null)
      {
        throw new Exception("Internal Error: Cannot locate strings to form message from.");
      }

      var localisedStrings = new List<string>();

      for (var i = 0; i < fields.Length; i++)
      {
        var field = fields[i];
        var stringAttributes = field.GetCustomAttributes(typeof(StringValueAttribute), false);
        if (stringAttributes.Length != 1)
        {
          throw new Exception($"Internal Error: Cannot locate 'StringValue' attribute for string with index {i}");
        }

        var defaultString = ((StringValueAttribute)stringAttributes[0]).StringValue;
        var localisedString = base.GetLocalisedString(defaultString);
        var value = (int)field.GetRawConstantValue();

        this.byLocalisedString[localisedString] = value;
        this.byValue[value] = localisedString;
        this.defaultStringsByValue[value] = defaultString;
        localisedStrings.Add(localisedString);
      }

      this.localisedStrings = new ReadOnlyCollection<string>(localisedStrings);
    }

    /// <summary>
    /// Don't allow this method to be used so developers are forced to use enums to define their strings.
    /// 
    /// This overrides the base.GetLocalisedString(string) method
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public new string GetLocalisedString(string defaultValue)
    {
      throw new Exception("This method cannot be used, please use the version where a string enum index is passed");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="defaultValueIndex"></param>
    /// <returns></returns>
    public string GetLocalisedString(int defaultValueIndex)
    {
      if (!this.byValue.TryGetValue(defaultValueIndex, out var localisedString))
      {
        throw new Exception($"Internal Error: Invalid index requested ({defaultValueIndex}) for enumeration {this.ownerTypeFullName}");
      }

      return localisedString;
    }

    public string GetLocalisedString(int defaultValueIndex, params object[] formatArgs)
    {
      var locString = this.GetLocalisedString(defaultValueIndex);
      return formatArgs == null ? locString : string.Format(locString, formatArgs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IList<string> GetAllLocalisedStrings()
    {
      return this.localisedStrings.ToList();
    }

    /// <summary>
    /// Returns the Enum id for a particualr StringValue associated with an element in the enumeration
    /// </summary>
    /// <returns></returns>
    public int GetEnumValueForLocalisedString(string localisedString)
    {
      if (!this.byLocalisedString.TryGetValue(localisedString, out var value))
      {
        throw new Exception($"Cannot locate item with localised string value of '{localisedString}' in enumerated type '{this.stringsEnumType.FullName}'");
      }

      return value;
    }

    public string GetUnLocalisedString(int defaultValueIndex)
    {
      if (!this.defaultStringsByValue.TryGetValue(defaultValueIndex, out var unLocalisedString))
      {
        throw new Exception($"Internal Error: Invalid index requested ({defaultValueIndex}) for enumeration {this.ownerTypeFullName}");
      }

      return JsonConvert.SerializeObject(new UnLocalisedString { Text = unLocalisedString });
    }

    public string GetUnLocalisedString(int defaultValueIndex, params object[] formatArgs)
    {
      if (!this.defaultStringsByValue.TryGetValue(defaultValueIndex, out var unLocalisedString))
      {
        throw new Exception($"Internal Error: Invalid index requested ({defaultValueIndex}) for enumeration {this.ownerTypeFullName}");
      }

      return JsonConvert.SerializeObject(new UnLocalisedString { Text = unLocalisedString, Arguments = formatArgs });
    }

    public string LocaliseUnlocalisedString(string value)
    {
      var unLocalisedString = JsonConvert.DeserializeObject<UnLocalisedString>(value);

      var localisedString = base.GetLocalisedString(unLocalisedString.Text);
      return unLocalisedString.Arguments == null ? localisedString : string.Format(localisedString, unLocalisedString.Arguments);
    }

    private class UnLocalisedString
    {
      public string Text { get; set; }

      public object[] Arguments { get; set; }
    }
  }
}