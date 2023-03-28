using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AMCS.Data.Indexing.DelimitedData
{
  public class IndexedDelimitedDataContainerFactory<ContainerType, RecordType, IndexType> : DelimitedDataContainerFactory<ContainerType,RecordType>
    where ContainerType : IIndexedDelimitedDataContainer<IndexType>, new()
    where RecordType : ISerializableArray, new()
    where IndexType : IRangeIndex, new()
  {

    public Func<RecordType, string> GetIndexKey { get; private set; }

    public IndexedDelimitedDataContainerFactory(Func<RecordType, string> getIndexKey) 
      : base()
    {
      GetIndexKey = getIndexKey;
    }

    public IndexedDelimitedDataContainerFactory(string fieldDelimeter, string fieldDelimeterEscapeCharacter, string recordDelimeter, string recordDelimeterEscapeCharacter, Func<RecordType, string> getIndexKey) :
      base(fieldDelimeter, fieldDelimeterEscapeCharacter, recordDelimeter, recordDelimeterEscapeCharacter)
    {
      GetIndexKey = getIndexKey;
    }


    /// <summary>
    /// Gets all records located under requested Index
    /// </summary>
    /// <param name="dataContainer">Delimited Data Container</param>
    /// <param name="indexName">Name of index to return Records for</param>
    /// <returns>Collection of Records limited by Index</returns>
    public IList<RecordType> GetRecords(ContainerType dataContainer, string indexName)
    {
      var applicableIndexes = dataContainer.Indexes.Where(i => i.Name == indexName);
      if(applicableIndexes.Count() == 0) { return new List<RecordType>(); }
      return GetRecords(dataContainer, applicableIndexes.Cast<IndexType>().ToList());
    }

    /// <summary>
    /// Gets all records located under requested Index
    /// </summary>
    /// <param name="dataContainer">Delimited Data Container</param>
    /// <param name="indexName">Name of index to return Records for</param>
    /// <returns>Collection of Records limited by Index</returns>
    public IList<RecordType> GetRecords(ContainerType dataContainer, string[] indexNames)
    {
      var applicableIndexes = dataContainer.Indexes.Where(i => indexNames.Contains(i.Name));
      if (applicableIndexes.Count() == 0) { return new List<RecordType>(); }
      return GetRecords(dataContainer, applicableIndexes.Cast<IndexType>().ToList());
    }

    /// <summary>
    /// Gets all records located under provided Indexes
    /// </summary>
    /// <param name="dataContainer">Delimited Data Container</param>
    /// <param name="forTheseIndexes">Indexes to return Records for</param>
    /// <returns>Collection of Records limited by Index</returns>
    public IList<RecordType> GetRecords(ContainerType dataContainer, IList<IndexType> forTheseIndexes)
    {
      List<RecordType> output = new List<RecordType>();
      foreach (IndexType index in forTheseIndexes)
      {
        output.AddRange(GetRecords(dataContainer, index));
      }
      return output;
    }


    /// <summary>
    /// Gets all records located under provided Index
    /// </summary>
    /// <param name="dataContainer">Delimited Data Container</param>
    /// <param name="forTheseIndexes">Indexe to return Records for</param>
    /// <returns>Collection of Records limited by Index</returns>
    public IList<RecordType> GetRecords(ContainerType dataContainer, IndexType forThisIndex)
    {
      List<RecordType> output = new List<RecordType>();
      string searchString = dataContainer.Data.Substring(Convert.ToInt32(forThisIndex.Offset), Convert.ToInt32(forThisIndex.Length));
      string[] stringRecords = Utils.SplitString(searchString
            .Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter)
            , dataContainer.RecordDelimiter, true);
      foreach (string stringRecord in stringRecords)
      {
        string unescapedStringRecord = stringRecord.Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter);
        output.Add(DelimitedDataSerializer<RecordType>.Parse(unescapedStringRecord, dataContainer.FieldDelimiter, dataContainer.FieldDelimiterEscapeCharacter));
      }
      return output;
    }


    /// <summary>
    /// Gets all records located under provided Index using a Stream as Data Source
    /// </summary>
    /// <param name="dataContainer">DelimitedDataContainer that represents data in dataStream</param>
    /// <param name="dataStream">DelimitedDataContainer Data As a Stream</param>
    /// <param name="forTheseIndexes">Indexes to return Records for</param>
    /// <returns>Collection of Records limited by Index</returns>
    public IList<RecordType> GetRecordsFromStream(ContainerType dataContainer, Stream dataStream, IndexType forThisIndex)
    {
      List<RecordType> output = new List<RecordType>();

      //Ensure Index is compatible with Stream and this method's limits

      if (dataStream.Length > Int32.MaxValue)
      {
        throw new ArgumentOutOfRangeException("Data Stream exceeds maximum supported length");
      } 
      else if (dataStream.Length < forThisIndex.Offset + forThisIndex.Length)
      {
        throw new ArgumentOutOfRangeException("Index Offset and Length exceed length of Data Stream");
      }
      else if (forThisIndex.Offset < 0)
      {
        throw new ArgumentOutOfRangeException("Index Offset must be equal or greater than zero");
      }
      else if (forThisIndex.Length < 0)
      {
        throw new ArgumentOutOfRangeException("Index Length must be equal or greater than zero");
      }
      else if (forThisIndex.Offset > Int32.MaxValue)
      {
        throw new NotSupportedException("Index Offset greater than " + Int32.MaxValue + " is not supported");
      }
      else if (forThisIndex.Length > Int32.MaxValue)
      {
        throw new NotSupportedException("Index Length greater than " + Int32.MaxValue + " is not supported");
      }


      //Move to start of region of interest in Stream
      dataStream.Seek(forThisIndex.Offset, SeekOrigin.Begin);
      //Get reader from stream (but do NOT dispose! leave that to the calling method)
      StreamReader reader = new StreamReader(dataStream);
      //Build buffer to size expected
      char[] buffer = new char[forThisIndex.Length];
      reader.Read(buffer, 0, buffer.Length); //Count=buffer.Length could become a problem here if number is greater than Int32.MaxValue
      //Convert to string
      string searchString = new string(buffer);
      //Extract records
      string[] stringRecords = Utils.SplitString(searchString
            .Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter)
            , dataContainer.RecordDelimiter, true);
      foreach (string stringRecord in stringRecords)
      {
        string unescapedStringRecord = stringRecord.Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter);
        output.Add(DelimitedDataSerializer<RecordType>.Parse(unescapedStringRecord, dataContainer.FieldDelimiter, dataContainer.FieldDelimiterEscapeCharacter));
      }
      return output;
    }

    /// <summary>
    /// Gets all records located between two Indexes using a Stream as Data Source
    /// </summary>
    /// <param name="dataContainer">DelimitedDataContainer that represents data in dataStream</param>
    /// <param name="dataStream">DelimitedDataContainer Data As a Stream</param>
    /// <param name="startingAtThisIndex">First Index to start returning Records for</param>
    /// <param name="endingAtThisIndex">Last Index to return Records for</param>
    /// <returns>Collection of Records limited by Index</returns>
    public IList<RecordType> GetRecordsFromStream(ContainerType dataContainer, Stream dataStream, IndexType startingAtThisIndex, IndexType endingAtThisIndex)
    {
      List<RecordType> output = new List<RecordType>();

      //Ensure Index is compatible with Stream and this method's limits
      if (dataStream.Length > Int32.MaxValue)
      {
        throw new ArgumentOutOfRangeException("Data Stream exceeds maximum supported length");
      }
      else if (dataStream.Length < startingAtThisIndex.Offset + startingAtThisIndex.Length || dataStream.Length < endingAtThisIndex.Offset + endingAtThisIndex.Length)
      {
        throw new ArgumentOutOfRangeException("Index Offset and Length exceed length of Data Stream");
      }
      else if (startingAtThisIndex.Offset < 0 || endingAtThisIndex.Offset < 0)
      {
        throw new ArgumentOutOfRangeException("Index Offset must be equal or greater than zero");
      }
      else if (startingAtThisIndex.Length < 0 || endingAtThisIndex.Length < 0)
      {
        throw new ArgumentOutOfRangeException("Index Length must be equal or greater than zero");
      }
      else if (startingAtThisIndex.Offset > Int32.MaxValue || endingAtThisIndex.Offset > Int32.MaxValue)
      {
        throw new NotSupportedException("Index Offset greater than " + Int32.MaxValue + " is not supported");
      }
      else if (startingAtThisIndex.Length > Int32.MaxValue || endingAtThisIndex.Length > Int32.MaxValue)
      {
        throw new NotSupportedException("Index Length greater than " + Int32.MaxValue + " is not supported");
      }


      long startAt = (startingAtThisIndex.Offset < endingAtThisIndex.Offset) ? startingAtThisIndex.Offset : endingAtThisIndex.Offset;
      long length = 0;
      if (startingAtThisIndex.Offset < endingAtThisIndex.Offset)
      {
        //startingAtThisIndex is located earlier in stream than endingAtThisIndex
        length = endingAtThisIndex.Offset + endingAtThisIndex.Length - startingAtThisIndex.Offset;
      }
      else
      {
        //endingAtThisIndex is located earlier in stream than startingAtThisIndex
        length = startingAtThisIndex.Offset + startingAtThisIndex.Length - endingAtThisIndex.Offset;
      }


      //Move to start of region of interest in Stream
      dataStream.Seek(startingAtThisIndex.Offset, SeekOrigin.Begin);
      //Get reader from stream (but do NOT dispose! leave that to the calling method)
      StreamReader reader = new StreamReader(dataStream);
      //Build buffer to size expected
      char[] buffer = new char[length];
      reader.Read(buffer, 0, buffer.Length); //Count=buffer.Length could become a problem here if number is greater than Int32.MaxValue
      //Convert to string
      string searchString = new string(buffer);
      //Extract records
      string[] stringRecords = Utils.SplitString(searchString
            .Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter)
            , dataContainer.RecordDelimiter, true);
      foreach (string stringRecord in stringRecords)
      {
        string unescapedStringRecord = stringRecord.Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter);
        output.Add(DelimitedDataSerializer<RecordType>.Parse(unescapedStringRecord, dataContainer.FieldDelimiter, dataContainer.FieldDelimiterEscapeCharacter));
      }
      return output;
    }


    public override ContainerType CreateFrom(RecordType[] recordsToContain)
    {
      StringBuilder csvDataBuilder = new StringBuilder();
      Dictionary<string, IndexType> indexes = new Dictionary<string,IndexType>(); //IRangeIndexes wrapped in dictionary for efficiency of looking up existing IndexNames
      foreach (RecordType record in recordsToContain)
      {
        string escapedRecordAsString =
          DelimitedDataSerializer<RecordType>.Serialize(record, FieldDelimeter, FieldDelimeterEscapeCharacter)
            .Replace(RecordDelimeter, RecordDelimeterEscapeCharacter);
        long recordStartsAt = csvDataBuilder.Length;
        csvDataBuilder.Append(escapedRecordAsString);
        csvDataBuilder.Append(RecordDelimeter);
        long recordEndsAt = csvDataBuilder.Length;

        if (GetIndexKey != null)
        {
          string indexName = GetIndexKey(record);
          if (indexes.ContainsKey(indexName))
          {
            //Increase length of index to include this record too
            indexes[indexName].Length = recordEndsAt - indexes[indexName].Offset;
          }
          else
          {
            //Create new Index as no existing index with this key created yet
            indexes.Add(GetIndexKey(record), new IndexType() { Name = indexName, Offset = recordStartsAt, Length = recordEndsAt - recordStartsAt });
          }
        }
      }

      return new ContainerType()
      {
        FieldDelimiter = FieldDelimeter,
        FieldDelimiterEscapeCharacter = FieldDelimeterEscapeCharacter,
        RecordDelimiter = RecordDelimeter,
        RecordDelimiterEscapeCharacter = RecordDelimeterEscapeCharacter,
        Indexes = indexes.Select(i => i.Value).ToArray(), //Output all created Indexes
        Data = csvDataBuilder.ToString()
      };
    }



  }
}
