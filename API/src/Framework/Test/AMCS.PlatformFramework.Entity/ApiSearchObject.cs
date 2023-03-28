namespace AMCS.PlatformFramework.Entity
{
  using AMCS.Data.Entity;
  using System;
  using System.Runtime.Serialization;

  /// <summary>
  /// To be serialised correctly when pass to API all objects must inherit from EntityObject and override the below properties.
  /// Since they are unused simply inherit from this abstract class instead and it will reduce code required in every API object class.
  /// </summary>
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public abstract class ApiSearchObject : EntityObject
  {
    #region overrides

    public override int? GetId()
    {
      return null;
    }
    /// <summary>
    /// Returns table name used to generate update/insert
    /// </summary>
    /// <returns>table name.</returns>
    public override string GetTableName()
    {
      return "NA";
    }

    /// <summary>
    /// Returns primary key name used to generate update/insert
    /// </summary>
    /// <returns>primary key  name.</returns>
    public override string GetKeyName()
    {
      return "NA";
    }

    #endregion overrides
  }
}
