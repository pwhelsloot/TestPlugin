using System;
using System.Linq;
using System.Text;

namespace AMCS.Data.Indexing.DelimitedData
{
  public static class DelimitedDataSerializer<T> where T : ISerializableArray, new()
  {
    /// <summary>
    /// Escape sequence that cannot exist in any data to be parsed by this class
    /// </summary>
    public const string ESCAPE_SEQUENCE = "____\xff\x00\xff____";

    public static string Serialize(T objectToSerialize, string fieldDelimiter, string fieldDelimiterEscapeCharacter)
    {
      StringBuilder sb = new StringBuilder();
      foreach (string fieldValue in objectToSerialize.ToSerializedValues())
      {
        string escapedFieldValue = ((fieldValue == null) ? "" : fieldValue).Replace(fieldDelimiter, fieldDelimiterEscapeCharacter);
        sb.Append(escapedFieldValue);
        sb.Append(fieldDelimiter);
      }
      return sb.ToString();
    }

    public static T Parse(string objectAsString, string fieldDelimiter, string fieldDelimiterEscapeCharacter)
    {
      bool containsFieldDelimiterEscapeCharacter = objectAsString.Contains(fieldDelimiterEscapeCharacter);

      //ensure to use escape sequence that does not contain the fieldDelimiter
      string escapeSeq = ESCAPE_SEQUENCE.Replace(fieldDelimiter, "");

      if (containsFieldDelimiterEscapeCharacter)
      {
        //Escape all fieldDelimiterEscapeCharacters
        objectAsString = objectAsString.Replace(fieldDelimiterEscapeCharacter, escapeSeq);
      }

      string[] objectAsSerializedArray = Utils.SplitString(objectAsString, fieldDelimiter, false);

      if (containsFieldDelimiterEscapeCharacter)
      {
        //Replace escaped fieldDelimiters with the delimiter now
        objectAsSerializedArray = objectAsSerializedArray.Select(s => s.Replace(escapeSeq, fieldDelimiter)).ToArray();
      }

      T newObject = Activator.CreateInstance<T>();
      newObject.PopulateFromSerializedValues(objectAsSerializedArray);
      return newObject;
    }
  }
}
