using AMCS.Data.EndToEnd.Tests;
using AMCS.Data.Entity;
using TechTalk.SpecFlow;

namespace AMCS.Data.EndToEnd.Tests
{
  public interface IProfileBuilderManager
  {
    TBuilder GetProfileEntityByName<TEntity, TBuilder>(string name, ScenarioContext context)
      where TEntity : EntityObject
      where TBuilder : BaseBuilder;
  }
}