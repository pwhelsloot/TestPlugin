using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.Search;
using AMCS.Data.Server.SQL.Fetch;

namespace AMCS.Data.Server.SQL
{
  internal class SQLReadable : ISQLReadable
  {
    private readonly ISQLReaderFactory readerFactory;
    private readonly FetchInfo fetchInfo;
    private bool loadPropertyValues;
    private ISQLExecutableResult executableResult;

    public SQLReadable(ISQLReaderFactory readerFactory, FetchInfo fetchInfo)
    {
      this.readerFactory = readerFactory;
      this.fetchInfo = fetchInfo;
    }

    public DataTable GetDataTable()
    {
      using (var readerResult = readerFactory.GetReader())
      {
        var result = new DataTable();

        using (var reader = readerResult.GetReader())
        {
          result.Load(reader);
        }

        executableResult = readerResult.GetResult();

        return result;
      }
    }

    public ISQLExecutableResult GetResult()
    {
      if (executableResult == null)
        throw new InvalidOperationException("Results are not available until the query is executed");

      return executableResult;
    }

    public ISQLReadable LoadPropertyValues()
    {
      return LoadPropertyValues(true);
    }

    public ISQLReadable LoadPropertyValues(bool value)
    {
      loadPropertyValues = value;
      return this;
    }

    public IList List(Type type, IList list)
    {
      return List(type, list, null, null);
    }

    public IList List(Type type, IList list, Type childType, string childProperty)
    {
      ReadList(type, list, childType, childProperty);
      return list;
    }

    public List<T> List<T>()
    {
      return List<T>(typeof(T));
    }

    public List<T> List<T>(Type type)
    {
      var list = new List<T>();
      ReadList(type ?? typeof(T), list, null, null);
      return list;
    }

    public IList<T> List<T>(IList<T> list)
    {
      return List<T>(typeof(T), list);
    }

    public IList<T> List<T>(Type type, IList<T> list)
    {
      ReadList(type ?? typeof(T), (IList)list, null, null);
      return list;
    }

    public List<T> List<T>(Type childType, string childProperty)
    {
      return List<T>(typeof(T), childType, childProperty);
    }

    public List<T> List<T>(Type type, Type childType, string childProperty)
    {
      var list = new List<T>();
      ReadList(type ?? typeof(T), list, childType, childProperty);
      return list;
    }

    public IList<T> List<T>(IList<T> list, Type childType, string childProperty)
    {
      return List(typeof(T), list, childType, childProperty);
    }

    public IList<T> List<T>(Type type, IList<T> list, Type childType, string childProperty)
    {
      if (list == null)
        list = new List<T>();
      ReadList(type ?? typeof(T), (IList)list, childType, childProperty);
      return list;
    }

    public List<T> List<T>(Func<ISQLRecord, T> map)
    {
      var list = new List<T>();
      ReadListWithMapper(list, map);
      return list;
    }

    public void Read(Action<ISQLRecord> action)
    {
      ReadListWithCallback(action);
    }

    public object First(Type type)
    {
      return ReadSingle(SQLReaderMode.First, type);
    }

    public T First<T>()
    {
      return First<T>(typeof(T));
    }

    public T First<T>(Func<ISQLRecord, T> map)
    {
      return (T)ReadSingleWithMapper(SQLReaderMode.First, map);
    }

    public T First<T>(Type type)
    {
      return (T)ReadSingle(SQLReaderMode.First, type);
    }

    public object FirstOrDefault(Type type)
    {
      return ReadSingle(SQLReaderMode.FirstOrDefault, type);
    }

    public T FirstOrDefault<T>()
    {
      return FirstOrDefault<T>(typeof(T));
    }

    public T FirstOrDefault<T>(Type type)
    {
      return (T)ReadSingle(SQLReaderMode.FirstOrDefault, type);
    }

    public T FirstOrDefault<T>(Func<ISQLRecord, T> map)
    {
      return (T)ReadSingleWithMapper(SQLReaderMode.FirstOrDefault, map);
    }

    public object FirstScalar()
    {
      return ReadSingle(SQLReaderMode.FirstScalar, null);
    }

    public T FirstScalar<T>()
    {
      return (T)FirstScalar();
    }

    public object FirstOrDefaultScalar()
    {
      return ReadSingle(SQLReaderMode.FirstOrDefaultScalar, null);
    }

    public T FirstOrDefaultScalar<T>()
    {
      return (T)FirstOrDefaultScalar();
    }

    public object Single(Type type)
    {
      return ReadSingle(SQLReaderMode.Single, type);
    }

    public T Single<T>()
    {
      return Single<T>(typeof(T));
    }

    public T Single<T>(Type type)
    {
      return (T)ReadSingle(SQLReaderMode.Single, type);
    }

    public T Single<T>(Func<ISQLRecord, T> map)
    {
      return (T)ReadSingleWithMapper(SQLReaderMode.Single, map);
    }

    public object SingleOrDefault(Type type)
    {
      return ReadSingle(SQLReaderMode.SingleOrDefault, type);
    }

    public T SingleOrDefault<T>()
    {
      return SingleOrDefault<T>(typeof(T));
    }

    public T SingleOrDefault<T>(Type type)
    {
      return (T)ReadSingle(SQLReaderMode.SingleOrDefault, type);
    }

    public T SingleOrDefault<T>(Func<ISQLRecord, T> map)
    {
      return (T)ReadSingleWithMapper(SQLReaderMode.SingleOrDefault, map);
    }

    public object SingleScalar()
    {
      return ReadSingle(SQLReaderMode.SingleScalar, null);
    }

    public T SingleScalar<T>()
    {
      return (T)SingleScalar();
    }

    public object SingleOrDefaultScalar()
    {
      return ReadSingle(SQLReaderMode.SingleOrDefaultScalar, null);
    }

    public T SingleOrDefaultScalar<T>()
    {
      return (T)SingleOrDefaultScalar();
    }

    public SearchResultsEntity GetSearchResults(string searchResultId = null)
    {
      return ReadSearchResults(searchResultId);
    }

    public GridSearchResultsEntity GetGridSearchResults(string searchResultId = null, string[] ignoreColumns = null)
    {
      return ReadGridSearchResults(searchResultId, ignoreColumns, false);
    }

    public GridSearchResultsEntity GetDynamicGridSearchResults(string searchResultId = null)
    {
      return ReadGridSearchResults(searchResultId, null, true);
    }

    private SQLReader CreateReader()
    {
      return new SQLReader(readerFactory, loadPropertyValues);
    }

    private object ReadSingle(SQLReaderMode mode, Type type)
    {
      var reader = CreateReader();
      var result = reader.ReadSingle(mode, type);
      executableResult = reader.GetResult();
      return result;
    }

    private object ReadSingleWithMapper<T>(SQLReaderMode mode, Func<ISQLRecord, T> map)
    {
      var reader = CreateReader();
      var result = reader.ReadSingleWithMapper(mode, map);
      executableResult = reader.GetResult();
      return result;
    }

    private void ReadList(Type type, IList list, Type childType, string childProperty)
    {
      var reader = CreateReader();
      reader.ReadList(type, list, childType, childProperty, fetchInfo);
      executableResult = reader.GetResult();
    }

    private void ReadListWithMapper<T>(IList<T> list, Func<ISQLRecord, T> map)
    {
      var reader = CreateReader();
      reader.ReadListWithMapper(list, map);
      executableResult = reader.GetResult();
    }

    private void ReadListWithCallback(Action<ISQLRecord> action)
    {
      var reader = CreateReader();
      reader.ReadListWithCallback(action);
      executableResult = reader.GetResult();
    }

    private SearchResultsEntity ReadSearchResults(string searchResultId)
    {
      var reader = CreateReader();
      var result = reader.ReadSearchResults(searchResultId ?? readerFactory.DefaultSearchResultId);
      executableResult = reader.GetResult();
      return result;
    }

    private GridSearchResultsEntity ReadGridSearchResults(string searchResultId, string[] ignoreColumns, bool dynamicGrid)
    {
      var reader = CreateReader();
      var result = reader.ReadGridSearchResults(searchResultId ?? readerFactory.DefaultSearchResultId, ignoreColumns, dynamicGrid);
      executableResult = reader.GetResult();
      return result;
    }
  }
}
