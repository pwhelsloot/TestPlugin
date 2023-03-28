using System.Collections.Generic;

namespace AMCS.Data.Schema
{
  public interface IQueryableObject: IDatabaseObject
  {
    IList<IColumn> Columns { get; }
  }
}
