using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Newtonsoft.Json;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslTriggerRequest
  {
    [JsonProperty("a")]
    public BslAction Action { get; set; }

    [JsonProperty("et")]
    public string EntityType { get; set; }

    [JsonProperty("ds")]
    public IDataSession DataSession { get; set; }

    [JsonProperty("eo")]
    public EntityObject EntityObject { get; set; }

    [JsonProperty("i")]
    public int Id { get; set; }

    [JsonProperty("g")]
    public Guid? GUID { get; set; }

    [JsonProperty("txid")]
    public Guid? TransactionId { get; set; }
  }
}
