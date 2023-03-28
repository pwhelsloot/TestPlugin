
namespace AMCS.Data.Schema
{
  public interface IForeignKey: IKey
  {
    IKey ReferencedKey { get; }
  }
}
