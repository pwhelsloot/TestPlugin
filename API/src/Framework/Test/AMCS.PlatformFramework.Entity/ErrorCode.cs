namespace AMCS.PlatformFramework.Entity
{
  using AMCS.Data.Util.Extension;

  public enum ErrorCode
  {
    [StringValue("This is an error")]
    SomeUserError = 100,


    [StringValue("This is an error with parameters: {0}, {1}")]
    SomeUserErrorWithParameters = 200
  }
}
