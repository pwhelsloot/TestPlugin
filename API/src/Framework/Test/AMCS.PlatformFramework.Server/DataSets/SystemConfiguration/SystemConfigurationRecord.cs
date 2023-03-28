namespace AMCS.PlatformFramework.Server.DataSets.SystemConfiguration
{
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Util.Extension;
  using AMCS.PlatformFramework.Entity;

  [DataSet("SystemConfiguration", Strings.SystemConfiguration, EntityType = typeof(SystemConfigurationEntity))]
  public class SystemConfigurationRecord : DataSetRecord
  {
    [DataSetColumn(Strings.Id, IsKeyColumn = true)]
    public int Id { get; set; }

    [DataSetColumn(Strings.Name, IsMandatory = true, IsDisplayColumn = true)]
    public string Name { get; set; }

    [DataSetColumn(Strings.Value)]
    public string Value { get; set; }

    protected override int GetId() => Id;

    private enum Strings
    {
      [StringValue("SystemConfiguration")]
      SystemConfiguration,

      [StringValue("ID")]
      Id,

      [StringValue("Name")]
      Name,

      [StringValue("Value")]
      Value
    }
  }
}
