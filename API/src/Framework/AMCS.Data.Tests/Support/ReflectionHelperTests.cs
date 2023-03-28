namespace AMCS.Data.Tests.Support
{
  using System;
  using AMCS.Data.Support;
  using NUnit.Framework;

  public class ReflectionHelperTests
  {
    [SetUp]
    public void DefaultState()
    {
      ReflectionHelper.DisableILGeneration(false);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void GetDefaultValueFactory_Returns_Correct_Value(bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
      foreach (var property in typeof(SampleClass).GetProperties())
      {
        var propertyType = property.GetType();
        var defaultValue = ReflectionHelper.GetDefaultValueFactory(propertyType);
        var expectedValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        Assert.That(defaultValue.Invoke(), Is.EqualTo(expectedValue));
      }
    }

    [TestCase(0,false)]
    [TestCase(0, true)]
    [TestCase(75, false)]
    [TestCase(75, true)]
    [TestCase(9499, false)]
    [TestCase(9499, true)]
    public void GetPropertyGetter_Returns_Correct_Value(int intValue, bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
      var propertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty));
      var propertyGetter = ReflectionHelper.GetPropertyGetter(propertyInfo);
      Assert.That(propertyGetter, Is.Not.Null);

      var propertyInfoPlus1 = typeof(SampleClass).GetProperty(nameof(SampleClass.IntPropertyPlus1));
      var propertyGetterPlus1 = ReflectionHelper.GetPropertyGetter(propertyInfoPlus1);
      Assert.That(propertyGetterPlus1, Is.Not.Null);

      var sampleClass = new SampleClass() { IntProperty = intValue };
      var resultOfGet = propertyGetter.Invoke(sampleClass);
      Assert.That(resultOfGet, Is.EqualTo(sampleClass.IntProperty));
      var resultOfGetPlus1 = propertyGetterPlus1.Invoke(sampleClass);
      Assert.That(resultOfGetPlus1, Is.EqualTo(sampleClass.IntProperty + 1));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void GetPropertySetter_ForPropertyWithoutSet_Throws_NotSupportedException(bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
      var propertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.GetOnlyProperty));
      var propertySetter = ReflectionHelper.GetPropertySetter(propertyInfo);
      Assert.That(propertySetter, Is.Not.Null);

      var sampleClass = new SampleClass();
      Assert.Throws<NotSupportedException>(() => propertySetter.Invoke(sampleClass, string.Empty));
    }

    [TestCase(0,false)]
    [TestCase(15, false)]
    [TestCase(9999, false)]
    [TestCase(0, true)]
    [TestCase(15, true)]
    [TestCase(9999, true)]
    public void GetPropertySetter_Returns_Correct_Value(int intValue, bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
      var propertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty));
      var propertySetter = ReflectionHelper.GetPropertySetter(propertyInfo);
      Assert.That(propertySetter, Is.Not.Null);

      var propertyInfoPlus1 = typeof(SampleClass).GetProperty(nameof(SampleClass.IntPropertyPlus1));
      var propertySetterPlus1 = ReflectionHelper.GetPropertySetter(propertyInfoPlus1);
      Assert.That(propertySetterPlus1, Is.Not.Null);

      var sampleClass = new SampleClass();
      propertySetter.Invoke(sampleClass, intValue);
      Assert.That(sampleClass.IntProperty, Is.EqualTo(intValue));
      propertySetterPlus1.Invoke(sampleClass, intValue);
      Assert.That(sampleClass.IntProperty, Is.EqualTo(intValue + 1));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void GetEntityPropertySetter_Returns_Correct_Value(bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
      var propertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty));
      var propertySetter = ReflectionHelper.GetEntityPropertySetter(propertyInfo);
      Assert.That(propertySetter, Is.Not.Null);
      var sampleClass = new SampleClass();
      // Valid value
      propertySetter.Invoke(sampleClass, 2);
      Assert.That(sampleClass.IntProperty, Is.EqualTo(2));

      // Use null on non-nullable, should set to correct default value
      propertySetter.Invoke(sampleClass, null);
      Assert.That(sampleClass.IntProperty, Is.EqualTo(0));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void GetEntityPropertySetter_ForCharProperty_Will_CoerceFromString(bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
      var propertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.CharProperty));
      var propertySetter = ReflectionHelper.GetEntityPropertySetter(propertyInfo);
      Assert.That(propertySetter, Is.Not.Null);
      var sampleClass = new SampleClass();
      // Valid char value
      propertySetter.Invoke(sampleClass, 'c');
      Assert.That(sampleClass.CharProperty, Is.EqualTo('c'));

      // String value
      propertySetter.Invoke(sampleClass, "c");
      Assert.That(sampleClass.CharProperty, Is.EqualTo('c'));

      // String value with additional value
      propertySetter.Invoke(sampleClass, "characters");
      Assert.That(sampleClass.CharProperty, Is.EqualTo('c'));

      // Int value
      propertySetter.Invoke(sampleClass, 2);
      Assert.That(sampleClass.CharProperty, Is.EqualTo((char)2));
    }


    private class SampleClass
    {
      private int internalIntValue = 0;
      public int IntProperty { get => internalIntValue; set => internalIntValue = value; }
      public int IntPropertyPlus1 { get => internalIntValue + 1; set => internalIntValue = value + 1; }
      public string StringProperty { get; set; }
      public char CharProperty { get; set; }

      public string GetOnlyProperty { get; }
    }
  }
}
