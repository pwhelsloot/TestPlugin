using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.UnitTests
{
  [EntityTable("Duplicate", "DuplicateId", "DuplicateObject")]
  public class DuplicateEntity
  {
    public int? DuplicateId { get; set; }
  }
}