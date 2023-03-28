namespace AMCS.PlatformFramework.Server.Configuration
{
  using AMCS.Data.Entity;
  using AMCS.Data.Server;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Server.DataSets.SystemConfiguration;
  using AMCS.PlatformFramework.Server.DataSets.User;

  public partial class DataConfigurationExtensions
  {
    private static void DataSetsBuildMapper(IEntityObjectMapperBuilder builder)
    {
      builder
        .CreateMap<UserRecord, UserEntity>(cfg => cfg.Map(nameof(UserEntity.EmailAddress), p => p.MapFrom(p1 => p1.Email)))
        .CreateMap<UserEntity, UserRecord>(cfg => cfg.Map(nameof(UserRecord.Email), p => p.MapFrom(p1 => p1.EmailAddress)))
        .CreateMap<SystemConfigurationRecord, SystemConfigurationEntity>(cfg => cfg.Map(nameof(SystemConfigurationEntity.SystemConfigurationId), p => p.MapFrom(p1 => p1.Id)))
        .CreateMap<SystemConfigurationEntity, SystemConfigurationRecord>(cfg => cfg.Map(nameof(SystemConfigurationRecord.Id), p => p.MapFrom(p1 => p1.SystemConfigurationId)));
    }
  }
}
