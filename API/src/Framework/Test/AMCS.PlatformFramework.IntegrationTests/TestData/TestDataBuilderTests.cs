namespace AMCS.PlatformFramework.IntegrationTests.TestData
{
  using System;
  using AMCS.Data.Server.TestData;
  using NUnit.Framework;

  [TestFixture]
  public class TestDataBuilderFixture : TestBase
  {
    [Test]
    public void ClassWithNullSeedWillThrowException()
    {
      Assert.Throws<NullReferenceException>(() => new TestDataBuilder(null));
    }


    [Test]
    [TestCase("123")]
    [TestCase("amcsgroup.com")]
    public void ClassWithValidSeedWillConstruct(string seedValue)
    {
      Assert.IsNotNull(new TestDataBuilder(seedValue));
    }

    [Test]
    [TestCase("123")]
    [TestCase("amcsgroup.com")]
    [TestCase("sampleSeed")]
    public void RandomValueRepeatableUsingSameSeed(string seedValue)
    {
      var builderA = new TestDataBuilder(seedValue);
      var builderB = new TestDataBuilder(seedValue);

      Assert.AreEqual(builderA.NextRandom(), builderB.NextRandom());
      Assert.AreEqual(builderA.NextRandom(), builderB.NextRandom());
    }
  }
}
