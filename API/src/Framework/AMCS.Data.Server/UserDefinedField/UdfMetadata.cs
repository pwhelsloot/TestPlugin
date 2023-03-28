namespace AMCS.Data.Server.UserDefinedField
{
  using System.Collections.Generic;
  using AMCS.Data.Entity.UserDefinedField;

  internal class UdfMetadata : IUdfMetadata
  {
    public IList<IUdfNamespace> Namespaces { get; }

    public UdfMetadata(IList<IUdfNamespace> namespaces)
    {
      Namespaces = namespaces;
    }
  }
}