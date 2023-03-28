namespace AMCS.Data.Indexing.DelimitedData
{
  public interface ISerializableArray
  {
    void PopulateFromSerializedValues(string[] stringValues);
    string[] ToSerializedValues();
  }
}
