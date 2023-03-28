namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Reflection;


  public class TestDataConfigurationContainer
  {
    private readonly TestDataConfigurationAttribute TestDataConfigurationAttribute;

    public Type ConfigurationType { get; }
    public string DisplayName => this.TestDataConfigurationAttribute.DisplayName;

    public TestDataConfigurationContainer(Type configurationType)
    {
      this.ConfigurationType = configurationType;
      this.TestDataConfigurationAttribute = this.ConfigurationType.GetCustomAttribute<TestDataConfigurationAttribute>();
    }
  }
}
