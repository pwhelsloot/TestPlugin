using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class OperationRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly int id;
    private readonly string operationName;
    private readonly JObject data;

    public OperationRequest(string collectionName, int id, string operationName, JObject data)
    {
      this.collectionName = collectionName;
      this.id = id;
      this.operationName = operationName;
      this.data = data;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      // Resolve the operation handler.

      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);
      var operation = GetOperationHandler(metadata);

      // Get the different types the message service implements.

      var typeArguments = operation.GetGenericTypeArguments(typeof(IEntityObjectMessageService<,,>));

      var entityType = typeArguments[0];
      var requestType = typeArguments[1];
      var responseType = typeArguments[2];

      // Get the method to execute the operation.

      var perform = operation.GetMethod("Perform", BindingFlags.Public | BindingFlags.Instance);

      // Parse the request into the request message type.

      var parser = new MessageParser();

      var request = parser.ParseRequest(requestType, data);

      // Execute the operation.

      var operationService = Activator.CreateInstance(operation);

      object response;

      using (var session = BslDataSessionFactory.GetDataSession(user, false))
      using (var transaction = session.CreateTransaction())
      {
        var entity = session.GetById(user, entityType, id);

        // Exceptions in reflection are wrapped in a TargetInvocationException. Unwrap.

        try
        {
          response = perform.Invoke(operationService, new[] { user, entity, request, session });
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
          throw ex.InnerException;
        }

        transaction.Commit();
      }

      // Write the response into a stream and rewrite that into the target writer.

      parser.WriteResponse(json, responseType, response);
    }

    private Type GetOperationHandler(IEntityObjectMetadata metadata)
    {
      foreach (var operation in metadata.Operations)
      {
        if (operation.Name == operationName)
          return operation.Handler;
      }

      throw new BatchRequestException($"Cannot find operation {operationName} for collection {collectionName}");
    }
  }
}
