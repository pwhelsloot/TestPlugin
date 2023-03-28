namespace AMCS.Data
{
  using System.Globalization;

  public interface ISessionToken
  {
    string UserName { get; }

    string UserIdentity { get; }

    int UserId { get; }

    int CompanyOutletId { get; }

    bool IsOfflineModeEnabled { get; }

    string TenantId { get; }

    CultureInfo Language { get; }
  }
}
