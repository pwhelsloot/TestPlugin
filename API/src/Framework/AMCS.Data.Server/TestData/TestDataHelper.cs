namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using AMCS.Data.Configuration;

  public static class TestDataHelper
  {
    public static void Extract(TypeManager typeManager, out Type configuration, out List<Type> options)
    {
      Type extractedConfiguration = null;
      var extractedOptions = new List<Type>();
      // Get all classes that implement ITestDataGenerator<TConfig, TOptions>
      var implementedClasses = typeManager
             .GetTypes()
             .Where(type => type.IsClass && type.GetInterface(typeof(ITestDataGenerator<,>).Name.ToString()) != null);

      foreach (var type in implementedClasses)
      {
        var interfaceImplenetationGenerator = type.GetInterface(typeof(ITestDataGenerator<,>).Name);

        // Config class is first generic param on interface implementation
        var configType = interfaceImplenetationGenerator.GenericTypeArguments[0];
        if (extractedConfiguration == null)
          extractedConfiguration = configType;
        else if (!extractedConfiguration.Equals(configType))
          throw new ArgumentException("Cannot have more than one configuration type");

        // Options class is second generic param on interface implementation
        var optionsType = interfaceImplenetationGenerator.GenericTypeArguments[1];
        if (!extractedOptions.Contains(optionsType))
          extractedOptions.Add(interfaceImplenetationGenerator.GenericTypeArguments[1]);
      }

      if (extractedConfiguration == null)
        throw new ArgumentException("You must register a configuration class for test data generation");

      if (extractedOptions.Count == 0)
        throw new ArgumentException("You must register at least one options class for test data generation");

      TestDataValidator.ValidateConfiguration(extractedConfiguration);
      configuration = extractedConfiguration;
      ValidateOptionTypes(extractedOptions);
      options = ValidateOptionDependencies(extractedOptions);
    }

    private static List<Type> ValidateOptionTypes(List<Type> optionTypes)
    {
      List<Type> optionRecords = new List<Type>();
      foreach (Type optionType in optionTypes)
      {
        TestDataValidator.ValidateOptionType(optionType);
        var testDataOptionsAttribute = optionType.GetCustomAttribute<TestDataOptionsAttribute>();
        var recordType = testDataOptionsAttribute.DataSetRecordType;
        if (testDataOptionsAttribute.DependendantTypes.Any() && testDataOptionsAttribute.DependendantTypes.Contains(recordType))
          throw new ArgumentException($"{optionType.Name} - Cannot define {recordType.Name} as a dependancy of itself in {nameof(TestDataOptionsAttribute)}");
        if (optionRecords.Contains(recordType))
          throw new ArgumentException($"{recordType.Name} is defined in more that one {nameof(TestDataOptionsAttribute)}");
        optionRecords.Add(recordType);
      }

      return ValidateOptionDependencies(optionTypes);
    }

    private static List<Type> ValidateOptionDependencies(List<Type> optionTypes)
    {
      var sorted = new List<Type>();
      var visited = new Dictionary<Type, bool>();

      var registeredOptions = new Dictionary<Type, Type>();
      foreach (var optionType in optionTypes)
      {
        registeredOptions.Add(optionType.GetCustomAttribute<TestDataOptionsAttribute>().DataSetRecordType, optionType);
      }
      foreach (var type in optionTypes.OrderBy(t => t.Name))
      {
        Visit(type, sorted, visited, registeredOptions);
      }
      return sorted;
    }

    private static void Visit(Type type, List<Type> sorted, Dictionary<Type, bool> visited, Dictionary<Type, Type> registered)
    {
      bool inProcess;
      var alreadyVisited = visited.TryGetValue(type, out inProcess);
      if (alreadyVisited)
      {
        if (inProcess)
          throw new ArgumentException($"Cyclic dependency found in {type.Name}");
      }
      else
      {
        visited[type] = true;
        var dependencies = GetDependencies(type);
        if (dependencies != null && dependencies.Count > 0)
        {
          foreach (var dependency in dependencies)
          {
            Visit(registered[dependency], sorted, visited, registered);
          }
        }
        visited[type] = false;
        sorted.Add(type);
      }
    }

    private static IList<Type> GetDependencies(Type type)
    {
      return type.GetCustomAttribute<TestDataOptionsAttribute>().DependendantTypes;
    }
  }
}
