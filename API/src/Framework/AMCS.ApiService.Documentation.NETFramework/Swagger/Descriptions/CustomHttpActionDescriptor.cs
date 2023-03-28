using System;
using System.Reflection;
using System.Web.Http.Controllers;

namespace AMCS.ApiService.Documentation.NETFramework.Swagger.Descriptions
{
  internal class CustomHttpActionDescriptor : ReflectedHttpActionDescriptor
  {
    public override Type ReturnType { get; }

    public string Version { get; }

    public CustomHttpActionDescriptor(HttpControllerDescriptor controllerDescriptor, MethodInfo methodInfo, Type responseType, string version)
      : base(controllerDescriptor, methodInfo)
    {
      Version = version;
      ReturnType = responseType ?? methodInfo.ReturnType;
    }
  }
}