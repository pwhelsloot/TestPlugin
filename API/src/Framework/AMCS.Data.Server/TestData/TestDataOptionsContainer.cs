namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;

  public class TestDataOptionsContainer
  {
    private readonly TestDataOptionsAttribute TestDataOptionsAttribute;

    public Type TestDataOptions { get; }

    public Type RecordType => this.TestDataOptionsAttribute.DataSetRecordType;
    public string DisplayName => this.TestDataOptionsAttribute.DisplayName;
    public IList<Type> DependsOnTypes => this.TestDataOptionsAttribute.DependendantTypes;

    public TestDataOptionsContainer(Type testDataOptions)
    {
      this.TestDataOptions = testDataOptions;
      this.TestDataOptionsAttribute = this.TestDataOptions.GetCustomAttribute<TestDataOptionsAttribute>();
    }
  }
}
