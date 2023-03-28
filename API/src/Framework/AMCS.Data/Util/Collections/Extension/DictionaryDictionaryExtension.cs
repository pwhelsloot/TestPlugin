namespace AMCS.Data.Util.Collections.Extension
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Extension methods for the Dictionary
  /// </summary>
  public static class IDictionaryExtensions
  {
    /// <summary>
    /// Tries to find a key from a value returns first one
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary">dictionary being searched</param>
    /// <param name="value">value lookign for</param>
    /// <param name="key">out the key found</param>
    /// <returns>true if match false otherwise</returns>
    public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey key)
    {
       key = default(TKey);
       foreach (KeyValuePair<TKey, TValue> pair in dictionary)
       {
           if (value.Equals(pair.Value))
           {
               key = pair.Key;
               return true;
           }
       }
       return false;
    }
  }
}
