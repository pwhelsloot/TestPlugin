namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  /// <summary>
  /// Determines visibility for a Configuration
  /// </summary>
  public enum ConfigurationVisibility
  {
    /// <summary>
    /// Only visible to the API
    /// </summary>
    Internal,

    /// <summary>
    ///  Queryable by the UI
    /// </summary>
    Public
  }
}
