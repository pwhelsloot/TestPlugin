namespace AMCS.Data.Entity.Glossary
{
  using System;
  using Newtonsoft.Json;

  [Serializable]
  [EntityTable("GlossaryInternalCache", nameof(GlossaryInternalCacheId))]
  public class ApiGlossary : CacheCoherentEntity
  {
    [EntityMember]
    [JsonProperty("glossaryId")]
    public int? GlossaryInternalCacheId { get; set; }
  
    [EntityMember]
    [JsonProperty("original")]
    public string Original { get; set; }

    [EntityMember]
    [JsonProperty("translated")]
    public string Translated { get; set; }

    [EntityMember]
    [JsonProperty("languageCode")]
    public string LanguageCode { get; set; }

    public override int? GetId() => GlossaryInternalCacheId;
  }
}