namespace AMCS.Data.Util.Collections.Extension
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class IEnumerableExtension
  {
    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire List<T>. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisColl">'this' for the extension method</param>
    /// <param name="match">The Predicate<T> delegate that defines the conditions of the element to search for.</param>
    /// <returns>
    ///   The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.
    /// </returns>
    public static T Find<T>(this IEnumerable<T> thisColl, Predicate<T> match) where T : new()
    {
      if (match == null)
        throw new ArgumentNullException("The 'match' parameter cannot be null");
      foreach (T item in thisColl)
      {
        if (match(item))
          return item;
      }
      return new T();
    }
    
    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns true if finds one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisColl">'this' for the extension method</param>
    /// <param name="match">The Predicate<T> delegate that defines the conditions of the element to search for.</param>
    /// <returns>
    ///   true if found; false if not found
    /// </returns>
    public static bool Exists<T>(this IEnumerable<T> thisColl, Predicate<T> match) where T : new()
    {
      if (match == null)
        throw new ArgumentNullException("The 'match' parameter cannot be null");
      foreach (T item in thisColl)
      {
        if (match(item))
          return true;
      }
      return false;
    }
  }
}
