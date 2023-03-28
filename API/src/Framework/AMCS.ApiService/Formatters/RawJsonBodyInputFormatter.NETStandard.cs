#if !NETFRAMEWORK
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Formatters
{
  public class RawJsonBodyInputFormatter : InputFormatter
  {
    public RawJsonBodyInputFormatter()
    {
      this.SupportedMediaTypes.Add("application/json");
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
      var request = context.HttpContext.Request;
      using (var reader = new StreamReader(request.Body))
      {
        var content = await reader.ReadToEndAsync();
        RawJsonBody body = new RawJsonBody(content);
        return await InputFormatterResult.SuccessAsync(body);
      }
    }

    protected override bool CanReadType(Type type)
    {
      return type == typeof(RawJsonBody);
    }
  }
}
#endif