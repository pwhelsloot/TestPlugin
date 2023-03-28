using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AMCS.PlatformFramework.Tests
{
  [TestFixture]
  public class TestFixture : TestBase
  {
    [Test]
    public void SimpleTest()
    {
      bool isTrue = true;
      Assert.IsTrue(isTrue);
    }
  }
}
