using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public abstract class DataSetRecord : IDataSetRecord
  {
    private Guid? referenceKey;

    int IDataSetRecord.GetId() => GetId();

    protected abstract int GetId();

    Guid? IDataSetRecord.GetReferenceKey() => referenceKey;

    void IDataSetRecord.SetReferenceKey(Guid? value) => referenceKey = value;
  }
}
