using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import.Planner
{
  internal class Plan
  {
    public List<DataSet> LoadOrder { get; } = new List<DataSet>();

    public List<DataSetRelationship> DelayedForeignKeys { get; } = new List<DataSetRelationship>();
  }
}
