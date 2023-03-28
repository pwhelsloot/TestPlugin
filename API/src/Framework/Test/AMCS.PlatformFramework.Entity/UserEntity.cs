using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity
{
  [EntityTable("User", "UserId")]
  [Serializable]
  public class UserEntity : EntityObject
  {
    [EntityMember]
    public int? UserId { get; set; }

    [EntityMember]
    public string UserName { get; set; }

    [EntityMember]
    public string EmailAddress { get; set; }

    [EntityMember]
    public string Password { get; set; }

    public override int? GetId() => UserId;

    private static readonly string[] ValidatedProperties =
    {
      nameof(UserName),
      nameof(Password)
    };

    public override string[] GetValidatedProperties() => ValidatedProperties;

    protected override string GetValidationError(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(UserName):
          if (string.IsNullOrWhiteSpace(UserName))
            return "User name cannot be empty";
          break;
        case nameof(Password):
          if (string.IsNullOrWhiteSpace(Password))
            return "Password cannot be empty";
          break;
      }

      return null;
    }
  }
}
