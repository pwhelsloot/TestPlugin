using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;
using AMCS.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class MessageRequest : IBatchRequest
  {
    private readonly string name;
    private readonly JObject data;

    public MessageRequest(string name, JObject data)
    {
      this.name = name;
      this.data = data;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      // Resolve the operation handler.

      var operation = DataServices.Resolve<IEntityObjectMetadataManager>().FindOperation(name);
      if (operation == null)
        throw new BatchRequestException($"Cannot find operation of name {name}");

      // Get the different types the message service implements.

      var typeArguments = operation.Handler.GetGenericTypeArguments(typeof(IMessageService<,>));

      var requestType = typeArguments[0];
      var responseType = typeArguments[1];

      // Get the method to execute the operation.

      var perform = operation.Handler.GetMethod("Perform", BindingFlags.Public | BindingFlags.Instance);

      // Parse the request into the request message type.

      var parser = new MessageParser();

      var request = parser.ParseRequest(requestType, data);

      // Execute the operation.

      var operationService = Activator.CreateInstance(operation.Handler);

      object response;

      // Exceptions in reflection are wrapped in a TargetInvocationException. Unwrap.

      try
      {
        response = perform.Invoke(operationService, new[] { user, request });
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }

      // Write the response into a stream and rewrite that into the target writer.

      parser.WriteResponse(json, responseType, response);
    }
  }
}
