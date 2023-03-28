using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface IEntityObjectEntityMapperBuilder<TFrom, TTo>
  {
    IEntityObjectEntityMapperBuilder<TFrom, TTo> Map(string property, Action<IEntityObjectPropertyMapperBuilder<TFrom, TTo>> configure);

    IEntityObjectEntityMapperBuilder<TFrom, TTo> MapColumn(string column, Action<IEntityObjectPropertyMapperBuilder<TFrom, TTo>> configure);

    IEntityObjectEntityMapperBuilder<TFrom, TTo> BeforeMap(Action<TFrom, TTo> action);

    IEntityObjectEntityMapperBuilder<TFrom, TTo> AfterMap(Action<TFrom, TTo> action);
  }
}
