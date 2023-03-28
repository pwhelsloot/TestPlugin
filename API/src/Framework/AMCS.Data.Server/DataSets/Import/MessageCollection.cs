using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Import
{
  /// <summary>
  /// Collection of messages.
  /// </summary>
  [JsonConverter(typeof(MessageCollectionConverter))]
  public class MessageCollection : IEnumerable<Message>
  {
    private readonly IList<Message> messages = new List<Message>();

    /// <summary>
    /// Gets the number of messages in the collection.
    /// </summary>
    public int Count => messages.Count;

    /// <summary>
    /// Gets whether there are any errors in the collection.
    /// </summary>
    public bool HasErrors { get; private set; }

    /// <summary>
    /// Gets whether there are any warnings in the collection.
    /// </summary>
    public bool HasWarnings { get; private set; }

    /// <summary>
    /// Gets the message by the specified index.
    /// </summary>
    /// <param name="index">The index of the message.</param>
    /// <returns>The message.</returns>
    public Message this[int index] => messages[index];

    /// <summary>
    /// Initializes a new <see cref="MessageCollection"/> instance.
    /// </summary>
    public MessageCollection()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="MessageCollection"/> instance.
    /// </summary>
    /// <param name="messages">The messages to populate the collection with.</param>
    public MessageCollection(IEnumerable<Message> messages)
    {
      AddRange(messages);
    }

    public void Clear()
    {
      messages.Clear();

      HasErrors = false;
      HasWarnings = false;
    }

    /// <summary>
    /// Adds a new message to the collection.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void Add(Message message)
    {
      switch (message.Type)
      {
        case MessageType.Error:
          HasErrors = true;
          break;
        case MessageType.Warning:
          HasWarnings = true;
          break;
      }

      messages.Add(message);
    }

    /// <summary>
    /// Adds a new message to the collection.
    /// </summary>
    /// <param name="type">The type of the message.</param>
    /// <param name="description">The message.</param>
    /// <param name="dataSet">The data set associated with the message.</param>
    /// <param name="record">The record associated with the message.</param>
    /// <param name="exception">The exception associated with the message.</param>
    public void Add(MessageType type, string description, DataSet dataSet, IDataSetRecord record, MessageException exception = null)
    {
      Add(new Message(type, description, dataSet, record, exception));
    }

    /// <summary>
    /// Adds a range of messages to the collection.
    /// </summary>
    /// <param name="messages">The messages to add.</param>
    public void AddRange(IEnumerable<Message> messages)
    {
      foreach (var message in messages)
      {
        Add(message);
      }
    }

    /// <summary>
    /// Adds an error to the collection.
    /// </summary>
    /// <param name="description">The message.</param>
    /// <param name="dataSet">The data set associated with the message.</param>
    /// <param name="record">The record associated with the message.</param>
    /// <param name="exception">The exception associated with the message.</param>

    public void AddError(string description, DataSet dataSet = null, IDataSetRecord record = null, MessageException exception = null)
    {
      Add(MessageType.Error, description, dataSet, record, exception);
    }
    //done
    /// <summary>
    /// Adds a warning to the collection.
    /// </summary>
    /// <param name="description">The message.</param>
    /// <param name="dataSet">The data set associated with the message.</param>
    /// <param name="record">The record associated with the message.</param>
    /// <param name="exception">The exception associated with the message.</param>
    public void AddWarn(string description, DataSet dataSet = null, IDataSetRecord record = null, MessageException exception = null)
    {
      Add(MessageType.Warning, description, dataSet, record, exception);
    }

    /// <summary>
    /// Adds an information message to the collection.
    /// </summary>
    /// <param name="description">The message.</param>
    /// <param name="dataSet">The data set associated with the message.</param>
    /// <param name="record">The record associated with the message.</param>
    /// <param name="exception">The exception associated with the message.</param>
    public void AddInfo(string description, DataSet dataSet = null, IDataSetRecord record = null, MessageException exception = null)
    {
      Add(MessageType.Info, description, dataSet, record, exception);
    }

    /// <inheritdoc/>
    public IEnumerator<Message> GetEnumerator()
    {
      return messages.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
