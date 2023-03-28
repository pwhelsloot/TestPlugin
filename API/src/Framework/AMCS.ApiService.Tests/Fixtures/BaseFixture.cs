using Moq;

namespace AMCS.ApiService.Tests.Fixtures
{
  public class BaseFixture<TMock>
    where TMock : class
  {
    public Mock<TMock> Mock { get; }

    public BaseFixture(bool callBase = false)
    {
      Mock = new Mock<TMock>() { CallBase = callBase };
    }

    public TMock Create()
      => Mock.Object;
  }
}
