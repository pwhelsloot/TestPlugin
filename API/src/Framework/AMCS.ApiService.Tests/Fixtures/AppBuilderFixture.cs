using System;
using System.Collections.Generic;
using Moq;
using Owin;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class AppBuilderFixture : BaseFixture<IAppBuilder>
  {
    private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

    public AppBuilderFixture SetupProperties()
    {
      Mock.Setup(p => p.Properties)
        .Returns(properties);
      return this;
    }

    public AppBuilderFixture OnUse(Action<object, object[]> callback)
    {
      Mock.Setup(p => p.Use(It.IsAny<object>(), It.IsAny<object[]>()))
        .Callback(callback)
        .Returns(Mock.Object);
      return this;
    }
  }
}
