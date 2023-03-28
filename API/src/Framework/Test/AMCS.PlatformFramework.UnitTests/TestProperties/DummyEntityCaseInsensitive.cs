using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.UnitTests
{
  [EntityTable("DummyEntityCaseInsensitive", "DummyId", ObjectName ="DummyEntityCaseInsensitive")]
  public class DummyEntityCaseInsensitiveEntity : EntityObject
  {
    [EntityMember]
    public int? DummyId { get; set; }
  }
}