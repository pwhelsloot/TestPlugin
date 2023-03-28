namespace AMCS.ApiService.Support
{
  using Data.Server.Services;

  public static class UserServiceExtensions
  {
    public static string GetApplicationCookieName(this IUserService self)
    {
      return $"Platform.{ self.ApplicationCode }.SessionToken";
    }
  }
}
