using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AMCS.ApiService.Elemos;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.Configuration;

namespace AMCS.PlatformFramework.IntegrationTests.RestApi
{
  public static class ApiFixtureUtils
  {
    public static byte[] GetControllerInternalBinaryOutput<T>(ISessionToken userId, Func<EntityObjectServiceInternalController<T>, ActionResult> func, string post = null)
      where T : EntityObject, new()
    {
      return ControllerOutput(userId, func, new EntityObjectServiceInternalController<T>(), post).GetOutput();
    }

    public static string GetControllerInternalOutput<T>(ISessionToken userId, Func<EntityObjectServiceInternalController<T>, ActionResult> func, string post = null)
      where T : EntityObject, new()
    {
      return Encoding.UTF8.GetString(GetControllerInternalBinaryOutput(userId, func, post));
    }

    public static byte[] GetControllerExternalBinaryOutput<T>(ISessionToken userId, Func<EntityObjectServiceExternalController<T>, ActionResult> func, string post = null)
      where T : EntityObject, new()
    {
      return ControllerOutput(userId, func, new EntityObjectServiceExternalController<T>(), post).GetOutput();
    }

    public static string GetControllerExternalOutput<T>(ISessionToken userId, Func<EntityObjectServiceExternalController<T>, ActionResult> func, string post = null)
      where T : EntityObject, new()
    {
      return Encoding.UTF8.GetString(GetControllerExternalBinaryOutput(userId, func, post));
    }

    public static byte[] GetControllerEntityMessageBinaryOutput<TService, TEntity, TRequest, TResponse>(ISessionToken userId, int id)
      where TService : IEntityObjectMessageService<TEntity, TRequest, TResponse>, new()
      where TEntity : EntityObject, new()
    {
      return ControllerOutput(userId, p => p.Perform(id), new EntityObjectMessageServiceController<TService, TEntity, TRequest, TResponse>(), string.Empty).GetOutput();
    }

    public static string GetControllerEntityMessageOutput<TService, TEntity, TRequest, TResponse>(ISessionToken userId, int id)
      where TService : IEntityObjectMessageService<TEntity, TRequest, TResponse>, new()
      where TEntity : EntityObject, new()
    {
      return Encoding.UTF8.GetString(GetControllerEntityMessageBinaryOutput<TService, TEntity, TRequest, TResponse>(userId, id));
    }

    private static CustomHttpResponse ControllerOutput<T>(ISessionToken userId, Func<T, ActionResult> func, T controller, string post)
      where T : ControllerBase
    {
      // This sets up the controller with minimal infrastructure
      // to be able to execute entity controllers and get a response out.

      Stream stream = null;
      if (post != null)
        stream = new MemoryStream(new UTF8Encoding(false).GetBytes(post));

      var context = new ControllerContext(new CustomHttpContext(new CustomHttpRequest(stream)), new RouteData(), controller);
      context.HttpContext.SetAuthenticatedUser(userId);

      controller.ControllerContext = context;

      var result = func(controller);

      result.ExecuteResult(context);

      return (CustomHttpResponse)context.HttpContext.Response;
    }

    private class CustomHttpContext : HttpContextBase
    {
      public CustomHttpContext(HttpRequestBase request)
      {
        Request = request;
      }

      public override IDictionary Items { get; } = new Dictionary<object, object>();

      public override HttpRequestBase Request { get; }

      public override HttpResponseBase Response { get; } = new CustomHttpResponse();
    }

    private class CustomHttpRequest : HttpRequestBase
    {
      public CustomHttpRequest(Stream inputStream)
      {
        InputStream = inputStream;
      }

      public override Stream InputStream { get; }

      public override NameValueCollection Headers { get; } = new NameValueCollection();
    }

    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Suppressed for the template project only.")]
    private class CustomHttpResponse : HttpResponseBase
    {
      private TextWriter output;

      public override string ContentType { get; set; }

      public override Encoding ContentEncoding { get; set; }

      public override TextWriter Output
      {
        get
        {
          if (output == null)
            output = new StreamWriter(OutputStream);
          return output;
        }
      }

      public override Stream OutputStream { get; } = new MemoryStream();

      public byte[] GetOutput()
      {
        if (output != null)
        {
          output.Dispose();
          output = null;
        }

        return ((MemoryStream)OutputStream).ToArray();
      }
    }
  }
}
