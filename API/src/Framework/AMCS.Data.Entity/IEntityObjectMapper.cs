using System;
using System.Collections;
using System.Collections.Generic;
using AMCS.Data.Entity;

namespace AMCS.Data.Entity
{
  public interface IEntityObjectMapper
  {
    T Map<T>(object value);

    object Map(object value, Type targetType);

    void Map(object value, object target);

    IList<T> MapList<T>(IEnumerable values);

    IList<T> MapList<T>(IEnumerable values, Type targetType);

    IList MapList(IEnumerable values, Type targetType);

    void MapList(IEnumerable values, Type targetType, IList target);

    void MapArray(Array values, Type targetType, Array target);

    bool TryMapProperty(EntityObjectProperty property, Type targetType, out EntityObjectProperty result);
  }
}
