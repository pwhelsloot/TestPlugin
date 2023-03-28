namespace AMCS.Data.Server.Glossary
{
  using System.Collections.Generic;
  using AMCS.Data.Entity.Glossary;
  using AMCS.Data.Server.Services;
  
  public interface IGlossaryCacheService : ICacheCoherentEntityService<ApiGlossary>
  {
    IList<ApiGlossary> GetGlossaries();
  }
}