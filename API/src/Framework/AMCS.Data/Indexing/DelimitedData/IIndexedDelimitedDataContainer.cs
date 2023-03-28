namespace AMCS.Data.Indexing.DelimitedData
{
  public interface IIndexedDelimitedDataContainer<IndexType> : IDelimitedDataContainer
    where IndexType : IRangeIndex
  {
    IndexType[] Indexes { get; set; }
  }
}
