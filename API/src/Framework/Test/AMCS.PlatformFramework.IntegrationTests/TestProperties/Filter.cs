using AMCS.ApiService.Filtering;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.IntegrationTests.TestProperties
{
  internal static class Filter
  {
    private static readonly EntityObjectAccessor FilterAccessor = EntityObjectAccessor.ForType(typeof(FilterEntity));

    public static void AssertFilter(string filter, Action<ICriteria> callback)
    {
      var actual = CriteriaFilterParser.Parse(filter, FilterAccessor);

      var expected = Criteria.For(typeof(FilterEntity));
      callback(expected);

      Assert.AreEqual(expected.Expressions.Count, actual.Expressions.Count);

      for (int i = 0; i < expected.Expressions.Count; i++)
      {
        var actualExpression = actual.Expressions[i];
        var expectedExpression = expected.Expressions[i];

        switch (expectedExpression)
        {
          case IFieldExpression expectedField:
            var actualField = (IFieldExpression)actualExpression;
            Assert.AreEqual(expectedField.Field, actualField.Field);
            Assert.AreEqual(expectedField.Comparison, actualField.Comparison);
            if (!(expectedField.Value is string) && actualField.Value is IEnumerable)
              AssertCollectionsEqual((IEnumerable)expectedField.Value, (IEnumerable)actualField.Value);
            else
              Assert.AreEqual(expectedField.Value, actualField.Value);
            break;

          case ISearchExpression expectedSearch:
            var actualSearch = (ISearchExpression)actualExpression;
            Assert.AreEqual(expectedSearch.Search, actualSearch.Search);
            break;

          default:
            Assert.Fail();
            break;
        }
      }
    }

    public static void Fails(string filter)
    {
      Fails<FilterException>(filter);
    }

    public static void Fails<T>(string filter)
      where T : Exception
    {
      Assert.Throws<T>(() => CriteriaFilterParser.Parse(filter, FilterAccessor));
    }

    private static void AssertCollectionsEqual(IEnumerable expectedList, IEnumerable actualList)
    {
      var expected = expectedList.Cast<object>().ToList();
      var actual = actualList.Cast<object>().ToList();

      Assert.AreEqual(expected.Count, actual.Count);

      for (int i = 0; i < expected.Count; i++)
      {
        Assert.AreEqual(expected[i], actual[i]);
      }
    }
  }
}