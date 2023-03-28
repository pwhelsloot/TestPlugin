namespace AMCS.Data.Indexing.DelimitedData
{
  public interface IDelimitedDataContainer
  {
    string FieldDelimiter { get; set; }
    string FieldDelimiterEscapeCharacter { get; set; }
    string RecordDelimiter { get; set; }
    string RecordDelimiterEscapeCharacter { get; set; }
    string Data { get; set; }
  }
}
