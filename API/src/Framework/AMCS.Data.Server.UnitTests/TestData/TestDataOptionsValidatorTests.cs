namespace AMCS.Data.Server.UnitTests.TestData
{
  using System;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class TestDataOptionsValidationFixture
  {
    [Test]
    public void ClassWithValidPropertyTypesWillPassValidation()
    {
      Assert.DoesNotThrow(() => TestDataValidator.ValidateOptionType(typeof(ValidClass)));
    }

    [Test]
    public void ClassWithNoTestDataAttributeWillFailValidation()
    {
      Assert.Throws<InvalidOperationException>(() => TestDataValidator.ValidateOptionType(typeof(InvalidClassNoAttribute)));
    }

    [Test]
    public void ClassWithInvalidPropertyTypesWillFailValidation()
    {
      Assert.Throws<InvalidOperationException>(() => TestDataValidator.ValidateOptionType(typeof(InvalidClassUnsupportedType)));
    }

    private abstract class BaseValidClass
    {
      public int IntProperty { get; } = 1;
      public int? IntNullableProperty { get; } = null;
      public string StringProperty { get; } = "Test";
      public decimal DecimalProperty { get; } = 1.0M;
      public decimal? DecimalNullableProperty { get; } = null;
      public bool BoolProperty { get; } = true;
      public bool? BoolNullableProperty { get; } = null;
    }

    private class DummyDataSetRecord : IDataSetRecord
    {
      public int GetId()
      {
        throw new NotImplementedException();
      }

      public Guid? GetReferenceKey()
      {
        throw new NotImplementedException();
      }

      public void SetReferenceKey(Guid? value)
      {
        throw new NotImplementedException();
      }
    }

    [TestDataOptions("Valid", typeof(DummyDataSetRecord))]
    private class ValidClass : BaseValidClass
    {
    }

    private class InvalidClassNoAttribute : BaseValidClass
    {
    }

    [TestDataOptions("UnsupportedType", typeof(DummyDataSetRecord))]
    private class InvalidClassUnsupportedType : BaseValidClass
    {
      public DateTime InvalidDateTimeroperty { get; } = DateTime.Now;
    }
  }
}
