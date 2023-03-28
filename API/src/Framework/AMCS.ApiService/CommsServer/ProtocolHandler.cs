using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.CommsServer.Serialization;
using AMCS.Data;
using AMCS.Data.Entity.CommsServer;
using AMCS.Data.Server;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.Services;
using AMCS.Data.Util.DateTime;
using AMCS.JobSystem.Scheduler;
using log4net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AMCS.ApiService.CommsServer
{
  public abstract class ProtocolHandler : ICommsServerProtocol, IDisposable
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(ProtocolHandler));

    private readonly string protocol;

    private ICommsServerClient client;
    private TelemetryClient telemetryClient;
    private bool disposed;

    public ICommsServerClient Client => client;

    protected ProtocolHandler(ICommsServerClient client)
    {
      this.client = client;
      protocol = GetType().GetCustomAttribute<CommsServerProtocolAttribute>()?.Protocol;
      telemetryClient = new TelemetryClient(new TelemetryConfiguration(DataServices.Resolve<IServerConfiguration>().ApplicationInsightsInstrumentationKey));  
    }

    public void ProcessMessages(IList<Message> messages, CancellationToken cancellationToken)
    {
      foreach (var message in messages)
      {
        ProcessMessage(message, cancellationToken);
      }
    }

    public virtual void StateChanged(CommsServerConnectionState connectionState)
    {
    }

    public void ProcessMessage(Message message, CancellationToken cancellationToken)
    {
      using (var operation = TrackRequest(message))
      {
        try
        {
          DoProcessMessage(message, cancellationToken);
        }
        catch (Exception ex)
        {
          if (operation != null)
          {
            operation.Telemetry.Success = false;
            telemetryClient.TrackException(ex);
          }

          DoProcessMessageError(message, ex);

          if (ex is SchedulerException)
            throw;
        }
        finally
        {
          if (operation != null)
            telemetryClient.StopOperation(operation);
        }
      }
    }

    protected virtual void DoProcessMessageError(Message message, Exception exception)
    {
      Log.Warn($"Failed to import message '{message.Id}' of type '{message.Type}'", exception);

      var error = new ErrorLogEntity
      {
        Error = exception.Message,
        InnerError = exception.InnerException?.Message,
        StackTrace = GetStackTrace(exception),
        MessageId = message.Id,
        MessageType = message.Type,
        MessageBody = message.Body,
        MessageCorrelationId = message.CorrelationId,
        Timestamp = DateTimeUnspecified.Now,
        Protocol = protocol
      };

      try
      {
        var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

        using (var session = BslDataSessionFactory.GetDataSession())
        using (var transaction = session.CreateTransaction())
        {
          session.Save(userId, error);
          transaction.Commit();
        }
      }
      catch (Exception ex)
      {
        Log.Warn($"Failed to import error message '{message.Id}' of type '{message.Type}'", ex);
      }
    }

    private static string GetStackTrace(Exception exception)
    {
      var sb = new StringBuilder();

      while (true)
      {
        sb.Append(exception.Message).Append(" (").Append(exception.GetType()).AppendLine(")");
        if (exception.StackTrace != null)
          sb.AppendLine().Append(exception.StackTrace);

        exception = exception.InnerException;
        if (exception == null)
          break;

        sb.AppendLine().AppendLine("=== Caused by ===").AppendLine();
      }

      return sb.ToString();
    }

    protected abstract void DoProcessMessage(Message message, CancellationToken cancellationToken);

    public void Publish(params Message[] messages)
    {
      Publish((ICollection<Message>)messages);
    }

    public virtual void Publish(ICollection<Message> messages)
    {
      try
      {
        client.Publish(messages);
      }
      catch (Exception ex)
      {
        foreach (var message in messages)
        {
          DoProcessMessageError(message, ex);
        }

        throw;
      }
    }

    protected virtual Message DoCreateMessage(string type, string body)
    {
      return new Message(
        Message.NewId(),
        type,
        body,
        Activity.Current?.Id
      );
    }

    private IOperationHolder<RequestTelemetry> TrackRequest(Message message)
    {
      if (!telemetryClient.IsEnabled())
        return null;

      if (message.CorrelationId != null)
        ForceClearCurrentActivity();

      var requestTelemetry = new RequestTelemetry
      {
        Name = "COMMSMESSAGE " + message.Type,
        Properties =
         {
           { "Comms Server message type", message.Type },
           { "Comms Server protocol", protocol },
           { "Comms Server message ID", message.Id }
         }
      };

      if (message.CorrelationId != null)
      {
        requestTelemetry.Context.Operation.ParentId = message.CorrelationId;
        requestTelemetry.Context.Operation.Id = GetOperationId(message.CorrelationId);
      }

      return telemetryClient.StartOperation(requestTelemetry);
    }

    private void ForceClearCurrentActivity()
    {
      // You can't suspend and resume the current activity. We could stop the
      // current one, but that's not really right. Instead, we just want to get rid of
      // it. The code below will start a new activity, not carrying over the parent
      // by setting the parent ID. Stopping that activity will set the current to
      // null without impacting the current activity.

      if (Activity.Current != null)
        new Activity("").SetParentId("x").Start().Stop();
    }

    private static string GetOperationId(string id)
    {
      // Returns the root ID from the '|' to the first '.' if any.
      int rootEnd = id.IndexOf('.');
      if (rootEnd < 0)
        rootEnd = id.Length;

      int rootStart = id[0] == '|' ? 1 : 0;
      return id.Substring(rootStart, rootEnd - rootStart);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    public virtual void Dispose(bool disposing)
    {
      if (disposed)
        return;

      if (disposing)
      {
        if (telemetryClient != null)
        {
          telemetryClient.Flush();
          telemetryClient = null;
        }

        if (client != null)
        {
          client.Dispose(); // Client needs to implement IDisposable
          client = null;
        }
      }

      // dispose native components

      disposed = true;
    }
  }
}
