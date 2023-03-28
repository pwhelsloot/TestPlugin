namespace AMCS.Data.Server.Glossary
{
  using System.Collections.Generic;
  using AMCS.Data.Entity.Glossary;
  using AMCS.Data.Configuration;

  public interface IGlossaryService : IDelayedStartup
  {
    IList<ApiGlossary> GetGlossaries();
    string Translate(string input, string languageCode);
  }
}