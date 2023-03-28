using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMCS.ApiService.Support
{
  internal static class Inflector
  {
    private static readonly InternalData Data = new InternalData();

    public static string Pluralize(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");

      if (Data.Uncountables.Contains(value))
        return value;

      foreach (var rule in Data.PluralRules)
      {
        if (rule.Regex.Match(value).Success)
        {
          return rule.Regex.Replace(value, rule.Replacement);
        }
      }

      return value;
    }

    public static string Singularize(string value)
    {
      if (Data.Uncountables.Contains(value))
        return value;

      foreach (var rule in Data.SingularRules)
      {
        if (rule.Regex.Match(value).Success)
        {
          return rule.Regex.Replace(value, rule.Replacement);
        }
      }

      return value;
    }

    public static string Camelize(string value)
    {
      return Camelize(value, true);
    }

    public static string Camelize(string value, bool firstLetterUppercase)
    {
      if (firstLetterUppercase)
      {
        return
            Regex.Replace(
                Regex.Replace(value, "/(.?)", p => "::" + p.Groups[1].Value.ToUpperInvariant()),
                "(?:^|_)(.)",
                p => p.Groups[1].Value.ToUpperInvariant());
      }
      else
      {
        return
            value.Substring(0, 1).ToLowerInvariant() +
            Camelize(value.Substring(1));
      }
    }

    public static string Underscore(string value)
    {
      value = value.Replace("::", "/");
      value = Regex.Replace(value, "([A-Z]+)([A-Z][a-z])", p => p.Groups[1].Value + "_" + p.Groups[2].Value);
      value = Regex.Replace(value, "([a-z\\d])([A-Z])", p => p.Groups[1].Value + "_" + p.Groups[2].Value);
      value = value.Replace("-", "_");

      return value.ToLowerInvariant();
    }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "C# 7 language construct")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "C# 7 language construct")]
    private class InternalData
    {
      public readonly List<(Regex Regex, string Replacement)> PluralRules = new List<(Regex Regex, string Replacement)>();
      public readonly List<(Regex Regex, string Replacement)> SingularRules = new List<(Regex Regex, string Replacement)>();
      public readonly List<string> Uncountables = new List<string>();

      public InternalData()
      {
        Uncountables.Add("equipment");
        Uncountables.Add("information");
        Uncountables.Add("rice");
        Uncountables.Add("money");
        Uncountables.Add("species");
        Uncountables.Add("series");
        Uncountables.Add("fish");
        Uncountables.Add("sheep");

        AddPlural("$", "s", true);
        AddPlural("s$", "s");
        AddPlural("(ax|test)is$", "$1es");
        AddPlural("(octop|vir)us$", "$1i");
        AddPlural("(alias|status)$", "$1es");
        AddPlural("(bu)s$", "$1ses");
        AddPlural("(buffal|tomat)o$", "$1oes");
        AddPlural("([ti])um$", "$1a");
        AddPlural("sis$", "ses");
        AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
        AddPlural("(hive)$", "$1s");
        AddPlural("([^aeiouy]|qu)y$", "$1ies");
        AddPlural("(x|ch|ss|sh)$", "$1es");
        AddPlural("(matr|vert|ind)(?:ix|ex)$", "$1ices");
        AddPlural("([m|l])ouse$", "$1ice");
        AddPlural("^(ox)$", "$1en");
        AddPlural("(quiz)$", "$1zes");

        AddSingular("s$", string.Empty);
        AddSingular("(n)ews$", "$1ews");
        AddSingular("([ti])a$", "$1um");
        AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
        AddSingular("(^analy)ses$", "$1sis");
        AddSingular("([^f])ves$", "$1fe");
        AddSingular("(hive)s$", "$1");
        AddSingular("(tive)s$", "$1");
        AddSingular("([lr])ves$", "$1f");
        AddSingular("([^aeiouy]|qu)ies$", "$1y");
        AddSingular("(s)eries$", "$1eries");
        AddSingular("(m)ovies$", "$1ovie");
        AddSingular("(x|ch|ss|sh)es$", "$1");
        AddSingular("([m|l])ice$", "$1ouse");
        AddSingular("(bus)es$", "$1");
        AddSingular("(o)es$", "$1");
        AddSingular("(shoe)s$", "$1");
        AddSingular("(cris|ax|test)es$", "$1is");
        AddSingular("(octop|vir)i$", "$1us");
        AddSingular("(alias|status)es$", "$1");
        AddSingular("^(ox)en", "$1");
        AddSingular("(vert|ind)ices$", "$1ex");
        AddSingular("(matr)ices$", "$1ix");
        AddSingular("(quiz)zes$", "$1");

        AddIrregular("person", "people");
        AddIrregular("man", "men");
        AddIrregular("child", "children");
        AddIrregular("sex", "sexes");
        AddIrregular("move", "moves");
        AddIrregular("cow", "kine");
      }

      private void AddIrregular(string singular, string plural)
      {
        AddPlural(singular.Substring(0, 1).ToLower() + singular.Substring(1) + "$", plural.Substring(0, 1).ToLower() + plural.Substring(1));
        AddPlural(singular.Substring(0, 1).ToUpper() + singular.Substring(1) + "$", plural.Substring(0, 1).ToUpper() + plural.Substring(1));
        AddSingular(plural.Substring(0, 1).ToLower() + plural.Substring(1) + "$", singular.Substring(0, 1).ToLower() + singular.Substring(1));
        AddSingular(plural.Substring(0, 1).ToUpper() + plural.Substring(1) + "$", singular.Substring(0, 1).ToUpper() + singular.Substring(1));
      }

      private void AddPlural(string expression, string replacement)
      {
        AddPlural(expression, replacement, false);
      }

      private void AddPlural(string expression, string replacement, bool caseSensitive)
      {
        var re = caseSensitive ? new Regex(expression) : new Regex(expression, RegexOptions.IgnoreCase);

        PluralRules.Insert(0, (re, replacement));
      }

      private void AddSingular(string expression, string replacement)
      {
        AddSingular(expression, replacement, false);
      }

      private void AddSingular(string expression, string replacement, bool caseSensitive)
      {
        var re = caseSensitive ? new Regex(expression) : new Regex(expression, RegexOptions.IgnoreCase);

        SingularRules.Insert(0, (re, replacement));
      }
    }
  }
}
