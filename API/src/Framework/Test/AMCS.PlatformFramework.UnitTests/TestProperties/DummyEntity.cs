using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.UnitTests
{
  [EntityTable("DummyTable", "DummyId")]
  public class DummyEntity : EntityObject
  {
    [EntityMember]
    public int? DummyId { get; set; }
  }
}