namespace AMCS.PlatformFramework.IntegrationTests.Framework
{
  using AMCS.Data;
  using AMCS.Data.Server.Services;
  using NUnit.Framework;
  
  [TestFixture]
  public class TempFileServiceFixture : TestBase
  {
    [Test]
    public void RoundtripJson()
    {
      var poco = new Poco { Value = 42 };

      var service = DataServices.Resolve<ITempFileService>();

      string id = service.WriteJson(poco);
      var roundtrip = service.ReadJson<Poco>(id);
      service.DeleteFile(id);

      Assert.AreEqual(poco.Value, roundtrip.Value);
    }

    [Test]
    public void RoundtripString()
    {
      string str = "Hello world!";

      var service = DataServices.Resolve<ITempFileService>();

      string id = service.WriteString(str);
      var roundtrip = service.ReadString(id);
      service.DeleteFile(id);

      Assert.AreEqual(str, roundtrip);
    }

    [Test]
    public void RoundtripBytes()
    {
      var bytes = new byte[] { 1, 2, 3 };

      var service = DataServices.Resolve<ITempFileService>();

      string id = service.WriteBytes(bytes);
      var roundtrip = service.ReadBytes(id);
      service.DeleteFile(id);

      Assert.AreEqual(bytes, roundtrip);
    }

    private class Poco
    {
      public int? Value { get; set; }
    }
  }
}
