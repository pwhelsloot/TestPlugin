using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetRecord
  {
    int GetId();

    Guid? GetReferenceKey();

    void SetReferenceKey(Guid? value);
  }
}
