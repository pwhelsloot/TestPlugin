namespace AMCS.Data.Entity.UserDefinedField
{
  using System.Collections.Generic;

  public interface IUdfNamespace
  {
    string Name { get; }

    IList<IUdfField> Fields { get; }
  }
}