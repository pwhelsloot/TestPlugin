using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.CommsServer
{
  [Serializable]
  [EntityTable("QueueOut", "QueueOutId", SchemaName = "comms")]
  public class QueueOutEntity : EntityObject
  {
    [EntityMember]
    public int QueueOutId { get; set; }

    [EntityMember]
    public string MessageId { get; set; }

    [EntityMember]
    public string Type { get; set; }

    [EntityMember]
    public string Body { get; set; }

    [EntityMember]
    public string CorrelationId { get; set; }

    public override int? GetId()
    {
      return QueueOutId;
    }
  }
}
