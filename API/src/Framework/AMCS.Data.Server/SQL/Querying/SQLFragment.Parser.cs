// Parsing code taken from:
// https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/SqlCommand/SqlString.cs.
// https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/SqlCommand/Parser/SqlParserUtils.cs
// License: LGPL see https://github.com/nhibernate/nhibernate-core/blob/master/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  partial class SQLFragment
  {
    private static readonly object ParameterPlaceholder = new object();

    private static IEnumerable<object> ParseParts(string text)
    {
      if (string.IsNullOrEmpty(text)) yield break;

      int offset = 0;
      int maxOffset = text.Length;
      int partOffset = 0;

      while (offset < maxOffset)
      {
        var ch = text[offset];
        switch (ch)
        {
          case '?':      // Parameter marker
            if (offset > partOffset)
            {
              yield return text.Substring(partOffset, offset - partOffset);
            }
            yield return ParameterPlaceholder;
            partOffset = offset += 1;
            break;
          case '\'':      // String literals
          case '\"':      // ANSI quoted identifiers
          case '[':       // Sql Server quoted indentifiers
            offset += ReadDelimitedText(text, maxOffset, offset);
            continue;
          case '/':
            if (offset + 1 < maxOffset && text[offset + 1] == '*')
            {
              offset += ReadMultilineComment(text, maxOffset, offset);
              continue;
            }
            break;
          case '-':
            if (offset + 1 < maxOffset && text[offset + 1] == '-')
            {
              offset += ReadLineComment(text, maxOffset, offset);
              continue;
            }
            break;
        }

        offset++;
      }

      if (maxOffset > partOffset)
      {
        yield return text.Substring(partOffset, offset - partOffset);
      }
    }

    private static int ReadDelimitedText(string text, int maxOffset, int offset)
    {
      var startOffset = offset;
      char quoteEndChar;

      // Determine end delimiter
      var quoteChar = text[offset++];
      switch (quoteChar)
      {
        case '\'':
        case '"':
          quoteEndChar = quoteChar;
          break;
        case '[':
          quoteEndChar = ']';
          break;
        default:
          throw new SQLCriteriaException($"Quoted text cannot start with '{text[offset]}' character");
      }

      // Find end delimiter, but ignore escaped end delimiters
      while (offset < maxOffset)
      {
        if (text[offset++] == quoteEndChar)
        {
          if (offset >= maxOffset || text[offset] != quoteEndChar)
          {
            // Non-escaped delimiter char
            return offset - startOffset;
          }

          // Escaped delimiter char
          offset++;
        }
      }

      throw new SQLCriteriaException($"Cannot find terminating '{quoteEndChar}' character for quoted text.");
    }

    private static int ReadLineComment(string text, int maxOffset, int offset)
    {
      var startOffset = offset;

      offset += 2;
      for (; offset < maxOffset; offset++)
      {
        switch (text[offset])
        {
          case '\r':
          case '\n':
            return offset - startOffset;
        }
      }

      return offset - startOffset;
    }

    private static int ReadMultilineComment(string text, int maxOffset, int offset)
    {
      var startOffset = offset;
      offset += 2;

      var prevChar = '\0';
      for (; offset < maxOffset; offset++)
      {
        var ch = text[offset];
        if (ch == '/' && prevChar == '*')
        {
          return offset + 1 - startOffset;
        }

        prevChar = ch;
      }

      throw new SQLCriteriaException("Cannot find terminating \'*/\' string for multiline comment.");
    }

    private static int ReadWhitespace(string text, int maxOffset, int offset)
    {
      var startOffset = offset;

      offset++;
      while (offset < maxOffset)
      {
        if (!char.IsWhiteSpace(text[offset])) break;
        offset++;
      }

      var result = offset - startOffset;
      return result;
    }
  }
}
