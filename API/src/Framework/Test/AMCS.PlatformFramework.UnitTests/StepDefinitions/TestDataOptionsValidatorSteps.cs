using AMCS.Data.Server.DataSets;
using AMCS.Data.Server.TestData;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests
{
  [Binding]
  public class TestDataOptionsValidatorSteps
  {
    private const string INVALID_PROPRTY_TYPE = "INVALIDPROPERTYTYPE";
    private const string VALID_PROPRTY_TYPE = "VALIDPROPERTYTYPE";
    private const string NO_TEST_DATA = "NOTESTDATA";
    private const string TEST_STATUS_PASS = "PASS";
    private const string TEST_STATUS_FAIL = "FAIL";
    private Type TypeofTestData;
    private Exception exception;

    [Given(@"class with data (.*)")]
    public void GivenClassWithInvalidDataInvalidPropertyType(string typeofTestData)
    {
      switch (typeofTestData.ToUpperInvariant())
      {
        case INVALID_PROPRTY_TYPE:
          TypeofTestData = typeof(InvalidClassUnsupportedType);
          break;
        case VALID_PROPRTY_TYPE:
          TypeofTestData = typeof(ValidClass);
          break;
        case NO_TEST_DATA:
          TypeofTestData = typeof(InvalidClassNoAttribute);
          break;
      }
    }

    [When(@"class is validated with testdata validator")]
    public void WhenClassIsValidatedWithTestdataValidator()
    {
      try
      {
        TestDataValidator.ValidateOptionType(TypeofTestData);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    [Then(@"validation matches expected result (.*)")]
    public void ThenValidationMatchesExpectedResultPass(string expectedresult)
    {
      switch (expectedresult.ToUpperInvariant())
      {
        case TEST_STATUS_FAIL:
          Assert.AreEqual(typeof(InvalidOperationException), exception.GetType());
          break;
        case TEST_STATUS_PASS:
          Assert.IsNull(exception);
          break;
      }
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
