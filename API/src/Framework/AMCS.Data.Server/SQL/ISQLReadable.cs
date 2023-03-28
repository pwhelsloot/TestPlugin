using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.Search;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLReadable
  {
    DataTable GetDataTable();

    ISQLExecutableResult GetResult();

    ISQLReadable LoadPropertyValues();

    ISQLReadable LoadPropertyValues(bool value);

    IList List(Type type, IList list);

    IList List(Type type, IList list, Type childType, string childProperty);

    List<T> List<T>();

    List<T> List<T>(Type type);

    IList<T> List<T>(IList<T> list);

    IList<T> List<T>(Type type, IList<T> list);

    List<T> List<T>(Type childType, string childProperty);

    List<T> List<T>(Type type, Type childType, string childProperty);

    IList<T> List<T>(IList<T> list, Type childType, string childProperty);

    IList<T> List<T>(Type type, IList<T> list, Type childType, string childProperty);

    List<T> List<T>(Func<ISQLRecord, T> map);

    void Read(Action<ISQLRecord> action);

    object First(Type type);

    T First<T>();

    T First<T>(Type type);

    T First<T>(Func<ISQLRecord, T> map);

    object FirstOrDefault(Type type);

    T FirstOrDefault<T>();

    T FirstOrDefault<T>(Type type);

    T FirstOrDefault<T>(Func<ISQLRecord, T> map);

    object FirstScalar();

    T FirstScalar<T>();

    object FirstOrDefaultScalar();

    T FirstOrDefaultScalar<T>();

    object Single(Type type);

    T Single<T>();

    T Single<T>(Type type);

    T Single<T>(Func<ISQLRecord, T> map);

    object SingleOrDefault(Type type);

    T SingleOrDefault<T>();

    T SingleOrDefault<T>(Type type);

    T SingleOrDefault<T>(Func<ISQLRecord, T> map);

    object SingleScalar();

    T SingleScalar<T>();

    object SingleOrDefaultScalar();

    T SingleOrDefaultScalar<T>();

    SearchResultsEntity GetSearchResults(string searchResultId = null);

    GridSearchResultsEntity GetGridSearchResults(string searchResultId = null, string[] ignoreColumns = null);

    GridSearchResultsEntity GetDynamicGridSearchResults(string searchResultId = null);
  }
}
