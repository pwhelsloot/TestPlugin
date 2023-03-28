namespace AMCS.Data.Entity
{
  public interface ICacheCoherentEntity
  {
    bool IsEqualTo(object obj);
  }
}