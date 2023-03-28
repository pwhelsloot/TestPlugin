namespace AMCS.Data.Util.Collections.Extension
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;

  /// <summary>
  /// Extension methods for ObservableCollection<T>
  /// </summary>
  public static class ObservableCollectionExtension
  {
    /// <summary>
    /// Adds the elements of the specified collection to the end of the List<T>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisColl">'this' for the extension method</param>
    /// <param name="collection">
    ///   The collection whose elements should be added to the end of the List<T>. 
    ///   The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
    /// </param>
    public static void AddRange<T>(this ObservableCollection<T> thisColl, IEnumerable<T> collection) 
    {
      if (collection == null)
        throw new ArgumentNullException("The 'collections' parameter cannot be null");
      using (IEnumerator<T> enumer = collection.GetEnumerator())
      {
        while (enumer.MoveNext())
        {
          thisColl.Add(enumer.Current);
        }
      }
    }

    /// <summary>
    /// Converts an ObservableCollection of type 'S' to a new ObservableCollection of type 'T'
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisColl">The this coll.</param>
    /// <param name="converter">The converter.</param>
    /// <returns></returns>
    public static ObservableCollection<T> ConvertAll<S, T>(this ObservableCollection<S> thisColl, Converter<S, T> converter)
    {
      ObservableCollection<T> converted = new ObservableCollection<T>();
      foreach (S item in thisColl)
        converted.Add(converter.Invoke(item));
      return converted;

    }

    /// <summary>
    /// Removes all items from an Observablecollection matching the condition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisColl">The this coll.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public static ObservableCollection<T> RemoveAll<T>(this ObservableCollection<T> thisColl, Func<T, bool> condition)
    {
      var itemsToRemove = thisColl.Where(condition).ToList();

      foreach (var itemToRemove in itemsToRemove)
      {
        thisColl.Remove(itemToRemove);
      }
      return thisColl;
    }
  }
}
