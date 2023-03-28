namespace AMCS.Data.Entity.UserDefinedField
{
  using System.Collections.Generic;

  public interface IUdfMetadata
  {
    IList<IUdfNamespace> Namespaces { get; }
  }
}