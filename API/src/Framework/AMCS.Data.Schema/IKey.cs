
using System.Collections.Generic;
namespace AMCS.Data.Schema
{
  public interface IKey: IConstraint
  {
    string TableSchema { get; }
    string TableName { get; }
    IList<string> ColumnNames { get; }
  }
}
