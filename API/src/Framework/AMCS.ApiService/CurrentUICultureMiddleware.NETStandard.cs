#if !NETFRAMEWORK

namespace AMCS.ApiService
{
  using System.Globalization;
  using System.Linq;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Http;

  public class CurrentUICultureMiddleware
  {
    private readonly RequestDelegate next;
    public CurrentUICultureMiddleware(RequestDelegate next) => this.next = next;

    public async Task Invoke(HttpContext context)
    {
      string languageHeader = context.Request.Headers["Accept-Language"];

      if (languageHeader != null)
      {
        var language = languageHeader.Split(',')
          .Select(StringWithQualityHeaderValue.Parse)
          .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
          .First().Value;

        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
      }

      if (next != null) 
        await next(context);
    }
  }
}

#endif