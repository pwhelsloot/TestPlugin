using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AMCS.Data.Server.DataSets.Import
{
  /// <summary>
  /// Represents a message from an import or export.
  /// </summary>
  [DebuggerDisplay("Type = {Type}, Description = {Description}, DataSet = {DataSet}")]
  public class Message
  {
    /// <summary>
    /// Gets the message type.
    /// </summary>
    public MessageType Type { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the table associated with the message.
    /// </summary>
    public DataSet DataSet { get; }

    /// <summary>
    /// Gets the record associated with the message.
    /// </summary>
    public IDataSetRecord Record { get; }

    /// <summary>
    /// Gets the exception associated with the message.
    /// </summary>
    public MessageException Exception { get; }

    public Message(MessageType type, string description, DataSet dataSet, IDataSetRecord record, MessageException exception)
    {
      Type = type;
      Description = description;
      DataSet = dataSet;
      Record = record;
      Exception = exception;
    }
  }
}
