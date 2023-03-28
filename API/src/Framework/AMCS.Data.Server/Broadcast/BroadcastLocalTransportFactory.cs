using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Channels;
using AMCS.Data.Server.Broadcast.Client;
using AMCS.Data.Server.Broadcast.Receiver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastLocalTransportFactory : IBroadcastTransportFactory
  {
    private BroadcastClientTransport broadcastClientTransport;
    private BroadcastReceiverTransport broadcastReceiverTransport;
    private bool disposed;

    public BroadcastLocalTransportFactory()
    {
      broadcastClientTransport = new BroadcastClientTransport(this);
      broadcastReceiverTransport = new BroadcastReceiverTransport(this);
    }

    public IBroadcastClientTransport CreateClientTransport()
    {
      return broadcastClientTransport;
    }

    public IBroadcastReceiverTransport CreateReceiverTransport()
    {
      return broadcastReceiverTransport;
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (broadcastClientTransport != null)
        {
          broadcastClientTransport.Dispose();
          broadcastClientTransport = null;
        }
        
        if (broadcastReceiverTransport != null)
        {
          broadcastReceiverTransport.Dispose();
          broadcastReceiverTransport = null;
        }

        disposed = true;
      }
    }

    private class BroadcastClientTransport : IBroadcastClientTransport
    {
      private readonly object syncRoot = new object();

      private readonly BroadcastLocalTransportFactory owner;

      private List<BroadcastReceiveMessage> pendingMessages = new List<BroadcastReceiveMessage>();
      private bool disposed;

      public event EventHandler Opened;
      public event ExceptionEventHandler Closed;

      public BroadcastClientTransport(BroadcastLocalTransportFactory owner)
      {
        this.owner = owner;
      }

      private void BroadcastReceiverTransport_Opened(object sender, EventArgs e)
      {
        lock (syncRoot)
        {
          foreach (var pendingMessage in pendingMessages)
          {
            owner.broadcastReceiverTransport.ReceiveMessage(pendingMessage);
          }

          pendingMessages = null;
        }
      }

      public void Open()
      {
        owner.broadcastReceiverTransport.Opened += BroadcastReceiverTransport_Opened;
        OnOpened();
      }

      public void SendMessage<T>(BroadcastMessage<T> broadcastMessage)
      {
        var broadcastReceiveMessage = new BroadcastReceiveMessage
        {
          Type = broadcastMessage.Type,
          Data = new JRaw(JsonConvert.SerializeObject(broadcastMessage.Data))
        };

        lock (syncRoot)
        {
          if (pendingMessages != null)
          {
            pendingMessages.Add(broadcastReceiveMessage);
            return;
          }
        }

        owner.broadcastReceiverTransport.ReceiveMessage(broadcastReceiveMessage);
      }

      private void OnOpened() => Opened?.Invoke(this, EventArgs.Empty);
      private void OnClosed(ExceptionEventArgs e) => Closed?.Invoke(this, e);

      public void Dispose()
      {
        if (!disposed)
        {
          OnClosed(new ExceptionEventArgs(null));

          disposed = true;
        }
      }
    }

    private class BroadcastReceiverTransport : IBroadcastReceiverTransport
    {
      private readonly BroadcastLocalTransportFactory owner;
      private volatile CloseMode closeMode;
      private bool disposed;

      public event MessageReceivedEventHandler MessageReceived;
      public event EventHandler Opened;
      public event ExceptionEventHandler Closed;

      public BroadcastReceiverTransport(BroadcastLocalTransportFactory owner)
      {
        this.owner = owner;
      }

      public void Open()
      {
        OnOpened();
      }

      public void ReceiveMessage(BroadcastReceiveMessage broadcastReceiveMessage)
      {
        // If we're (partially) closed, drop incoming messages.
        if (closeMode.HasFlag(CloseMode.Receive))
          return;

        OnMessageReceived(new MessageReceivedEventArgs(broadcastReceiveMessage));
      }

      public void Close(CloseMode mode)
      {
        closeMode = mode;
      }

      private void OnMessageReceived(MessageReceivedEventArgs e) => MessageReceived?.Invoke(e);
      private void OnOpened() => Opened?.Invoke(this, EventArgs.Empty);
      private void OnClosed(ExceptionEventArgs e) => Closed?.Invoke(this, e);

      public void Dispose()
      {
        if (!disposed)
        {
          Close(CloseMode.Both);

          OnClosed(new ExceptionEventArgs(null));

          disposed = true;
        }
      }
    }
  }
}
