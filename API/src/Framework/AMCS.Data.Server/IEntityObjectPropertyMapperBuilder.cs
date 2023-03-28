using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface IEntityObjectPropertyMapperBuilder<out TFrom, out TTo>
  {
    void MapFrom(string property);

    void MapFromColumn(string column);

    void MapFrom(Func<TFrom, object> func);

    void MapFrom(Func<TFrom, TTo, object> func);

    void Ignore();
  }
}
