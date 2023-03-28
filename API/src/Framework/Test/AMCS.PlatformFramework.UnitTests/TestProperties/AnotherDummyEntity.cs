using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.UnitTests
{
  [EntityTable("AnotherDummyTable", "DummyId")]
  public class AnotherDummyEntity : EntityObject
  {
    [EntityMember]
    public int? DummyId { get; set; }
  }
}