using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration.Mapping.Translate;

namespace AMCS.Data.Server.DataSets
{
  internal static class DataSetUtil
  {
    public static string GetNameFromTypeName(Type type, string prefix, string postfix)
    {
      string typeName = type.Name;
      Debug.Assert(typeName.StartsWith(prefix) && typeName.EndsWith(postfix));
      return typeName.Substring(prefix.Length, typeName.Length - (prefix.Length + postfix.Length));
    }

    public static string GetLocalizedString(Type ownerType, object name)
    {
      switch (name)
      {
        case Enum enumValue:
          return new BusinessObjectStringTranslator(ownerType.FullName, enumValue.GetType()).GetLocalisedString((int)name);
        case string stringValue:
          return stringValue;
        default:
          throw new ArgumentException("Names must be a localization enum or a string");
      }
    }
  }
}
