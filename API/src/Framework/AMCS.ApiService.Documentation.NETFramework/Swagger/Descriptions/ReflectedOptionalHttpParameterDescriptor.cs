using System.Web.Http.Controllers;

namespace AMCS.ApiService.Documentation.NETFramework.Swagger.Descriptions
{
  internal class ReflectedOptionalHttpParameterDescriptor : ReflectedHttpParameterDescriptor
  {
    public override bool IsOptional => true;
  }
}