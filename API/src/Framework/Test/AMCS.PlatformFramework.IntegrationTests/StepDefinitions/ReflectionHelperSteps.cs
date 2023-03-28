using AMCS.Data.Support;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.Steps
{
  [Binding]
  public class ReflectionHelperSteps
  {
    private readonly IList<Func<object>> DefaultValue = new List<Func<object>>();
    private readonly IList<object?> ExpectedValue = new List<object?>();
    private PropertyInfo? PropertyInfo;
    private PropertyInfo? PropertyInfoPlus1;
    private Func<object, object>? PropertyGetter;
    private Func<object, object>? PropertyGetterPlus1;
    private Action<object, object>? PropertySetter;
    private Action<object, object>? PropertySetterPlus1;
    private SampleClass sampleClass = new SampleClass();
    private object? ExpectedIntValue;
    private object? ExpectedIntValuePlus1;
    private Exception exception = new Exception();
    private int ActualIntValue = 0;
    private int NullInputActualIntValue = 0;
    private int ActualIntValuePlus1 = 0;
    private int IntegerValue = 0;
    private char ActualCharValue;
    private string InputDataType = String.Empty;

    [SetUp]
    public void DefaultState()
    {
      ReflectionHelper.DisableILGeneration(false);
    }

    private class SampleClass
    {
      private int internalIntValue = 0;
      public int IntProperty { get => internalIntValue; set => internalIntValue = value; }
      public int IntPropertyPlus1 { get => internalIntValue + 1; set => internalIntValue = value + 1; }
      public string? StringProperty { get; set; }
      public char CharProperty { get; set; }
      public string? GetOnlyProperty { get; }
    }

    [Given(@"disableILGeneration (.*) or not")]
    public void GivenDisableILGenerationOrNot(bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
    }

    [When(@"GetPropertyGetter is called with int property (.*)")]
    public void WhenGetPropertyGetterIsCalledWithIntProperty(int intValue)
    {
      PropertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty));
      PropertyGetter = ReflectionHelper.GetPropertyGetter(PropertyInfo);
      PropertyInfoPlus1 = typeof(SampleClass).GetProperty(nameof(SampleClass.IntPropertyPlus1));
      PropertyGetterPlus1 = ReflectionHelper.GetPropertyGetter(PropertyInfoPlus1);
      sampleClass = new SampleClass() { IntProperty = intValue };
      ExpectedIntValue = PropertyGetter.Invoke(sampleClass);
      ExpectedIntValuePlus1 = PropertyGetterPlus1.Invoke(sampleClass);
    }

    [When(@"GetDefaultValueFactory is called")]
    public void WhenGetDefaultValueFactoryIsCalled()
    {
      foreach (var property in typeof(SampleClass).GetProperties())
      {
        var propertyType = property.GetType();
        DefaultValue.Add(ReflectionHelper.GetDefaultValueFactory(propertyType));
        ExpectedValue.Add(propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null);
      }
    }

    [When(@"GetPropertySetter is called without any property set")]
    public void WhenGetPropertySetterIsCalledWithoutAnyPropertySet()
    {
      try
      {
        PropertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.GetOnlyProperty));
        PropertySetter = ReflectionHelper.GetPropertySetter(PropertyInfo);
        sampleClass = new SampleClass();
        PropertySetter.Invoke(sampleClass, string.Empty);
      }
      catch (Exception ex) { exception = ex; }
    }

    [When(@"GetPropertySetter is called with int property (.*)")]
    public void WhenGetPropertySetterIsCalledWithIntProperty(int intValue)
    {
      IntegerValue = intValue;
      PropertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty));
      PropertySetter = ReflectionHelper.GetPropertySetter(PropertyInfo);
      PropertyInfoPlus1 = typeof(SampleClass).GetProperty(nameof(SampleClass.IntPropertyPlus1));
      PropertySetterPlus1 = ReflectionHelper.GetPropertySetter(PropertyInfoPlus1);
      sampleClass = new SampleClass();
      PropertySetter.Invoke(sampleClass, intValue);
      ActualIntValue = sampleClass.IntProperty;
      PropertySetterPlus1.Invoke(sampleClass, intValue);
      ActualIntValuePlus1 = sampleClass.IntProperty;
    }

    [When(@"GetEntityPropertySetter is called with int and null value")]
    public void WhenGetEntityPropertySetterIsCalledWithIntAndNullValue()
    {
      var propertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty));
      var propertySetter = ReflectionHelper.GetEntityPropertySetter(propertyInfo);
      Assert.That(propertySetter, Is.Not.Null);
      var sampleClass = new SampleClass();
      // Valid value
      propertySetter.Invoke(sampleClass, 2);
      ActualIntValue = sampleClass.IntProperty;
      // Use null on non-nullable, should set to correct default value
      propertySetter.Invoke(sampleClass, null);
      NullInputActualIntValue = sampleClass.IntProperty;
    }

    [When(@"GetEntityPropertySetter is called with input data type (.*) and value (.*)")]
    public void WhenGetEntityPropertySetterIsCalledWithInputDataTypeAndValue(string inputDataType, string inputValue)
    {
      PropertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.CharProperty));
      PropertySetter = ReflectionHelper.GetEntityPropertySetter(PropertyInfo);
      Assert.That(PropertySetter, Is.Not.Null);
      switch (inputDataType.ToUpperInvariant())
      {
        case "INTEGER":
          var intValue = Convert.ToInt32(inputValue);
          PropertySetter.Invoke(sampleClass, (char)intValue);
          break;
        default:
          PropertySetter.Invoke(sampleClass, inputValue);
          break;
      }
      ActualCharValue = sampleClass.CharProperty;
      InputDataType = inputDataType;
    }

    [Then(@"result matches (.*) expected result")]
    public void ThenResultMatchesExpectedResult(string expectedResult)
    {
      if (!InputDataType.ToUpperInvariant().Equals("INTEGER"))
        Assert.That(ActualCharValue, Is.EqualTo(char.Parse(expectedResult)));
      else
      {
        var intValue = Convert.ToInt32(expectedResult);
        Assert.That(ActualCharValue, Is.EqualTo((char)intValue));
      }
    }

    [Then(@"result matches expected result")]
    public void ThenResultMatchesExpectedResult()
    {
      if (DefaultValue.Count != 0)
      {
        for (int i = 0; i < DefaultValue.Count; i++)
          Assert.That(DefaultValue[i].Invoke(), Is.EqualTo(ExpectedValue[i]));
      }
      else if (PropertyGetter != null)
      {
        Assert.That(PropertyGetterPlus1, Is.Not.Null);
        Assert.That(ExpectedIntValue, Is.EqualTo(sampleClass.IntProperty));
        Assert.That(ExpectedIntValuePlus1, Is.EqualTo(sampleClass.IntProperty + 1));
      }
      else if (PropertySetter != null && PropertySetterPlus1 != null)
      {
        Assert.That(PropertySetter, Is.Not.Null);
        Assert.That(PropertySetterPlus1, Is.Not.Null);
        Assert.That(ActualIntValue, Is.EqualTo(IntegerValue));
        Assert.That(ActualIntValuePlus1, Is.EqualTo(IntegerValue + 1));
      }
      else if (ActualIntValue != 0)
      {
        Assert.That(ActualIntValue, Is.EqualTo(2));
        Assert.That(NullInputActualIntValue, Is.EqualTo(0));
      }
    }

    [Then(@"not supported exception is thrown")]
    public void ThenNotSupportedExceptionIsThrown()
    {
      Assert.AreEqual(typeof(NotSupportedException), exception.GetType());
    }
  }
}