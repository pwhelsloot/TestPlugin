namespace AMCS.PlatformFramework.Server.DataSets.User
{
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Util.Extension;
  using AMCS.PlatformFramework.Entity;

  [DataSet("User", Strings.User, EntityType = typeof(UserEntity))]
  public class UserRecord : DataSetRecord
  {
    [DataSetColumn(Strings.Id, IsKeyColumn = true)]
    public int UserId { get; set; }

    [DataSetColumn(Strings.UserName, IsMandatory = true, IsDisplayColumn = true)]
    public string UserName { get; set; }

    [DataSetColumn(Strings.Email)]
    public string Email { get; set; }

    [DataSetColumn(Strings.Password, IsMandatory = true)]
    public string Password { get; set; }

    protected override int GetId() => UserId;

    private enum Strings
    {
      [StringValue("User")]
      User,

      [StringValue("ID")]
      Id,

      [StringValue("UserName")]
      UserName,

      [StringValue("Email")]
      Email,

      [StringValue("Password")]
      Password
    }
  }
}
