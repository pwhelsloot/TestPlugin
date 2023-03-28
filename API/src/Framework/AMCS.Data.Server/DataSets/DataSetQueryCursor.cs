using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetQueryCursor
  {
    public int Id { get; }

    public DataSetQueryCursor(int id)
    {
      Id = id;
    }
  }
}
