using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [Serializable]
  public struct SerialisableKeyValuePair<K, V>
  {
    public K Key { get; set; }
    public V Value { get; set; }
  }
}