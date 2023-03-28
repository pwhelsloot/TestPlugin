using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Search
{
  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/elemos")]
  [KnownType(typeof(DateTimeOffset))]
  public class SearchResultsEntity
  {
    [DataMember]
    public string Id { get; set; }

    // key = name, value = type full name
    [DataMember]
    public SerialisableKeyValuePair<string, string>[] Columns { get; set; }

    [DataMember]
    public ObservableCollection<object[]> Data { get; set; }

    public SearchResultsEntity()
    {
      Data = new ObservableCollection<object[]>();
    }

    public int GetColumnIndex(string name)
    {
      if (Columns == null || Columns.Length == 0)
        throw new Exception("No columns available");
      for (int i = 0; i < Columns.Length; i++)
      {
        if (name.Equals(Columns[i].Key))
          return i;
      }
      return -1;
    }

    public SearchResultsEntity FilterDataByStringColumn(string columnName, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        int columnIndex = GetColumnIndex(columnName);
        SearchResultsEntity result = this.Clone();
        result.Data = new ObservableCollection<object[]>(this.Data.Where(p => (p[columnIndex] != null && (((string)p[columnIndex]).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0 || ((string)p[columnIndex]).Replace(" ", "").IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)))); 
        return result;
      }
      else
      {
        return this.Clone();
      }
    }

    public SearchResultsEntity Clone()
    {
      SearchResultsEntity result = new SearchResultsEntity();

      result.Columns = new SerialisableKeyValuePair<string, string>[this.Columns.Length];
      this.Columns.CopyTo(result.Columns, 0);
      result.Data = new ObservableCollection<object[]>(this.Data);
      result.Id = this.Id;

      return result;
    }
  }
}