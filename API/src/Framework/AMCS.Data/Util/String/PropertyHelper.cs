namespace AMCS.Data.Util.String
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using Extension;

  public static class PropertyHelper
  {
    /// <summary>
    /// Gets Property name (don't assume that property name will be a database key column name !!!!!! )
    /// </summary>
    /// <typeparam name="T">Class type - no need to provide any more (see example)</typeparam>
    /// <param name="propertyPointer">Property expression, for example (() => CallTypeRestrictionId)</param>
    /// <returns></returns>
    public static string GetPropertyName<T>(Expression<Func<T>> property)
    {
      return property.GetPropertyName();
    }
  }
}
