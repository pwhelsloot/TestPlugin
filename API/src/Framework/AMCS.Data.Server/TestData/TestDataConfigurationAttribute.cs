namespace AMCS.Data.Server.TestData
{
  using System;

  [AttributeUsage(AttributeTargets.Class)]
  public class TestDataConfigurationAttribute : Attribute
  {
    public string DisplayName { get; }

    public TestDataConfigurationAttribute(string displayName)
    {
      DisplayName = displayName;
    }
  }
}
