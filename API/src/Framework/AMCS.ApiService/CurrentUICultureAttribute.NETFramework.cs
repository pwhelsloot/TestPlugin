#if NETFRAMEWORK

namespace AMCS.ApiService
{
  using System.Globalization;
  using System.Linq;
  using System.Net.Http.Headers;
  using System.Web.Mvc;

  public class CurrentUICultureAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      string languageHeader = filterContext.HttpContext.Request.Headers["Accept-Language"];

      if (languageHeader != null)
      {
        var language = languageHeader.Split(',')
          .Select(StringWithQualityHeaderValue.Parse)
          .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
          .First().Value;

        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
      }

      base.OnActionExecuting(filterContext);
    }
  }
}

#endif