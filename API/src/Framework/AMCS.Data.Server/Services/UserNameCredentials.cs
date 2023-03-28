namespace AMCS.Data.Server.Services
{
  public class UserNameCredentials : ICredentials
  {
    public string UserName { get; }

    public string Tenant { get; }
    
    public UserNameCredentials(string userName, string tenant)
    {
      UserName = userName;
      Tenant = tenant;
    }
    
    public UserNameCredentials(string userName)
    { 
      UserName = userName;
    }
  }
}
