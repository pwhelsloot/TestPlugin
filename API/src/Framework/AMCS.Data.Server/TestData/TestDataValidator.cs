namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using AMCS.Data.Server.DataSets;

  public static class TestDataValidator
  {
    private static readonly Type[] Whitelist = new Type[] {
      typeof(int),
      typeof(int?),
      typeof(string),
      typeof(decimal),
      typeof(decimal?),
      typeof(bool),
      typeof(bool?)
    };

    public static void ValidateOptionType(Type optionType)
    {
      var testDataAttribute = optionType.GetCustomAttribute<TestDataOptionsAttribute>();
      if (testDataAttribute == null)
        throw new InvalidOperationException("Options requires a TestDataOptions attribute");
      else if (string.IsNullOrEmpty(testDataAttribute.DisplayName))
        throw new InvalidOperationException($"{optionType.Name} must specify {nameof(testDataAttribute.DisplayName)}");
      else if (!typeof(IDataSetRecord).IsAssignableFrom(testDataAttribute.DataSetRecordType))
        throw new ArgumentException($"{nameof(testDataAttribute.DataSetRecordType)} must implement {nameof(IDataSetRecord)}");

      ValidateProperties(optionType);
    }

    public static void ValidateConfiguration(Type type)
    {
      var testDataAttribute = type.GetCustomAttribute<TestDataConfigurationAttribute>();
      if (testDataAttribute == null)
        throw new InvalidOperationException("Configuration requires a TestDataConfiguration attribute");
      else if (string.IsNullOrEmpty(testDataAttribute.DisplayName))
        throw new InvalidOperationException($"{type.Name} must specify {nameof(testDataAttribute.DisplayName)}");

      ValidateProperties(type);
    }

    public static void ValidateProperties(Type type)
    {
      foreach (var property in type.GetProperties())
      {
        if (!TypeIsWhitelisted(property.PropertyType))
        {
          var sb = new StringBuilder();
          sb.AppendLine();
          sb.AppendLine($"{type.Name}.{ property.Name } is of type {property.PropertyType}");
          sb.AppendLine("This type is not a supported type for test data options or config");
          sb.AppendLine("Supported types are:");
          sb.AppendLine($"{ string.Join(Environment.NewLine, Whitelist.Select(item => item.FullName))}");
          throw new InvalidOperationException(sb.ToString());
        }
      }
    }

    private static bool TypeIsWhitelisted(Type type)
    {
      return Whitelist.Any(whiteListedType => whiteListedType.Equals(type));
    }
  }
}
