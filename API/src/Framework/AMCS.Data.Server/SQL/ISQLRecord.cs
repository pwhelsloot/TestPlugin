using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLRecord
  {
    int FieldCount { get; }

    string GetName(int index);

    object Get(string field);

    T Get<T>(string field);

    object Get(int index);

    T Get<T>(int index);

    T GetEntity<T>();

    T GetEntity<T>(bool loadPropertyValues);
  }
}
