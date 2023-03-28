namespace AMCS.Data.Server.UserDefinedField
{
  using System.Collections.Generic;
  using AMCS.Data.Entity.UserDefinedField;

  internal class UdfNamespace : IUdfNamespace
  {
    public string Name { get; }

    public IList<IUdfField> Fields { get; }

    public UdfNamespace(string name, IList<IUdfField> fields)
    {
      Name = name;
      Fields = fields;
    }
  }
}