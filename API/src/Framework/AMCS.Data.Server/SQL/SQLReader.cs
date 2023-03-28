#pragma warning disable 0618

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.Search;
using AMCS.Data.Server.SQL.Fetch;

namespace AMCS.Data.Server.SQL
{
  internal class SQLReader
  {
    private readonly ISQLReaderFactory readerFactory;
    private readonly bool loadPropertyValues;
    private ISQLExecutableResult executableResult;

    public SQLReader(ISQLReaderFactory readerFactory, bool loadPropertyValues)
    {
      this.readerFactory = readerFactory;
      this.loadPropertyValues = loadPropertyValues;
    }

    public ISQLExecutableResult GetResult()
    {
      return executableResult;
    }

    public object ReadSingle(SQLReaderMode mode, Type type)
    {
      using (var readerResult = readerFactory.GetReader())
      {
        object result;

        using (var reader = readerResult.GetReader())
        {
          if (!reader.Read())
          {
            if (!AllowNoResults(mode))
              throw new SQLRecordNotFoundException("Query did not return results");
            result = null;
          }
          else if (IsScalar(mode))
          {
            result = reader[0];
            if (result is DBNull)
              result = null;
          }
          else
          {
            result = DataRowToObjectConvertor.ConvertDataRowToDataContractObject(reader, type, loadPropertyValues);
          }

          if (AllowOnlySingleResult(mode) && reader.Read())
            throw new SQLRecordNotFoundException("Query returned more than one result");
        }

        executableResult = readerResult.GetResult();

        return result;
      }
    }

    public T ReadSingleWithMapper<T>(SQLReaderMode mode, Func<ISQLRecord, T> map)
    {
      using (var readerResult = readerFactory.GetReader())
      {
        T result;

        using (var reader = readerResult.GetReader())
        {
          if (!reader.Read())
          {
            if (!AllowNoResults(mode))
              throw new SQLRecordNotFoundException("Query did not return results");

            result = default(T);
          }
          else
          {
            result = map(new SQLRecord(reader));

            if (AllowOnlySingleResult(mode) && reader.Read())
              throw new SQLRecordNotFoundException("Query returned more than one result");
          }
        }

        executableResult = readerResult.GetResult();

        return result;
      }
    }

    public void ReadList(Type type, IList list, Type childType, string childProperty, FetchInfo fetchInfo)
    {
      using (var readerResult = readerFactory.GetReader())
      {
        using (var reader = readerResult.GetReader())
        {
          if (loadPropertyValues)
            Debug.Assert(childType == null && childProperty == null, "Cannot combine load property values and connect child");

          if (childType != null || childProperty != null)
          {
            DataRowToObjectConvertor.ConvertDataReaderToDataContractObjectList(reader, type, list, childType, childProperty, loadPropertyValues);
          }
          else if (fetchInfo != null)
          {
            var fetchReader = new FetchReader(fetchInfo, list);
            fetchReader.Read(reader);
          }
          else
          {
            DataRowToObjectConvertor.ConvertDataReaderToDataContractObjectList(reader, type, list, loadPropertyValues);
          }
        }

        executableResult = readerResult.GetResult();
      }
    }

    public void ReadListWithMapper<T>(IList<T> list, Func<ISQLRecord, T> map)
    {
      if (loadPropertyValues)
        throw new NotSupportedException("Cannot load property values with custom mapper");

      using (var readerResult = readerFactory.GetReader())
      {
        using (var reader = readerResult.GetReader())
        {
          var record = new SQLRecord(reader);

          while (reader.Read())
          {
            list.Add(map(record));
          }
        }

        executableResult = readerResult.GetResult();
      }
    }

    public void ReadListWithCallback(Action<ISQLRecord> action)
    {
      if (loadPropertyValues)
        throw new NotSupportedException("Cannot load property values with custom mapper");

      using (var readerResult = readerFactory.GetReader())
      {
        using (var reader = readerResult.GetReader())
        {
          var record = new SQLRecord(reader);

          while (reader.Read())
          {
            action(record);
          }
        }

        executableResult = readerResult.GetResult();
      }
    }

    public SearchResultsEntity ReadSearchResults(string searchResultId)
    {
      Debug.Assert(!loadPropertyValues, "Cannot combine load property values with search results query");

      using (var readerResult = readerFactory.GetReader())
      {
        SearchResultsEntity result;

        using (var reader = readerResult.GetReader())
        {
          result = new SQLSearchDataAccessProvider().GetSearchResultsFromReader(searchResultId, reader);
        }

        executableResult = readerResult.GetResult();

        return result;
      }
    }

    public GridSearchResultsEntity ReadGridSearchResults(string searchResultId, string[] ignoreColumns, bool dynamicGrid)
    {
      Debug.Assert(!loadPropertyValues, "Cannot combine load property values with search results query");

      using (var readerResult = readerFactory.GetReader())
      {
        GridSearchResultsEntity result;

        using (var reader = readerResult.GetReader())
        {
          if (dynamicGrid)
            result = new SQLSearchDataAccessProvider().GetDynamicGridSearchResultsFromReader(searchResultId, reader, ignoreColumns);
          else
            result = new SQLSearchDataAccessProvider().GetGridSearchResultsFromReader(searchResultId, reader, ignoreColumns);
        }

        executableResult = readerResult.GetResult();

        return result;
      }
    }

    private static bool IsScalar(SQLReaderMode mode)
    {
      switch (mode)
      {
        case SQLReaderMode.FirstScalar:
        case SQLReaderMode.FirstOrDefaultScalar:
        case SQLReaderMode.SingleScalar:
        case SQLReaderMode.SingleOrDefaultScalar:
          return true;
        default:
          return false;
      }
    }

    private static bool AllowNoResults(SQLReaderMode mode)
    {
      switch (mode)
      {
        case SQLReaderMode.FirstOrDefault:
        case SQLReaderMode.FirstOrDefaultScalar:
        case SQLReaderMode.SingleOrDefault:
        case SQLReaderMode.SingleOrDefaultScalar:
          return true;
        default:
          return false;
      }
    }

    private static bool AllowOnlySingleResult(SQLReaderMode mode)
    {
      switch (mode)
      {
        case SQLReaderMode.Single:
        case SQLReaderMode.SingleOrDefault:
        case SQLReaderMode.SingleScalar:
        case SQLReaderMode.SingleOrDefaultScalar:
          return true;
        default:
          return false;
      }
    }
  }
}
