using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.CommsServer
{
  [Serializable]
  [EntityTable("ErrorLog", "ErrorLogId", SchemaName = "comms")]
  public class ErrorLogEntity : EntityObject
  {
    [EntityMember]
    public int? ErrorLogId { get; set; }

    [EntityMember]
    public string Error { get; set; }

    [EntityMember]
    public string InnerError { get; set; }

    [EntityMember]
    public string StackTrace { get; set; }

    [EntityMember]
    public string MessageId { get; set; }

    [EntityMember]
    public string MessageType { get; set; }

    [EntityMember]
    public string MessageBody { get; set; }

    [EntityMember]
    public string MessageCorrelationId { get; set; }

    [EntityMember]
    public DateTimeOffset? Timestamp { get; set; }

    [EntityMember]
    public string Protocol { get; set; }

    public override int? GetId()
    {
      return ErrorLogId;
    }
  }
}
