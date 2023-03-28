namespace AMCS.Data.Server.TestData
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using AMCS.Data.Server.DataSets;

  [AttributeUsage(AttributeTargets.Class)]
  public class TestDataOptionsAttribute : Attribute
  {
    public string DisplayName { get; }

    public Type DataSetRecordType { get; }

    public IList<Type> DependendantTypes { get; }

    public TestDataOptionsAttribute(string DisplayName, Type DataSetRecordType, params Type[] DependendantTypes)
    {
      this.DisplayName = DisplayName;
      this.DataSetRecordType = DataSetRecordType;
      ValidateTypeImplementsIDataSetRecord(DataSetRecordType);
      if (DependendantTypes == null || DependendantTypes.Length == 0)
        DependendantTypes = new Type[0];
      this.DependendantTypes = new ReadOnlyCollection<Type>(DependendantTypes);
      foreach (Type type in this.DependendantTypes)
      {
        ValidateTypeImplementsIDataSetRecord(type);
      }
    }

    public TestDataOptionsAttribute(string DisplayName, Type DataSetRecordType) : this(DisplayName, DataSetRecordType, null)
    { }

    private void ValidateTypeImplementsIDataSetRecord(Type type)
    {
      if (!typeof(IDataSetRecord).IsAssignableFrom(type))
        throw new ArgumentException($"{nameof(type)} must implement {nameof(IDataSetRecord)}");
    }
  }
}
