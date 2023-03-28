#if !NETFRAMEWORK

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace AMCS.Data.Server.Services
{
  internal class StaticFilesService
  {
    public StaticFilesService(IAppSetupService setupService, string filesPath, string outputPath)
    {
      filesPath = Path.GetFullPath(filesPath);
      ///Register SpaStaticFiles options
      setupService.RegisterConfigureServices(app => app.AddSpaStaticFiles(config =>
      {
        config.RootPath = filesPath;
      }));

      setupService.RegisterConfigure(app =>
      {
        //Fires SpaStaticFiles whenever a request starts with provided outputPath
        app.Map(new PathString(outputPath), client =>
        {
          //No need to configure options for UseSpaStaticFiles, they are provided by .AddSpaStaticFiles
          client.UseSpaStaticFiles();
          client.UseSpa(spa =>
          {
            spa.Options.SourcePath = outputPath;
            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
              OnPrepareResponse = context =>
              {
                if (context.File.Name == "index.html")
                {
                  context.Context.Response.Headers
                    .Add("Cache-Control", "no-cache, no-store, must-revalidate");
                  context.Context.Response.Headers.Add("Pragma", "no-cache");
                  context.Context.Response.Headers.Add("Expires", "-1");
                }
              }
            };
          });
        });
      });
    }
  }
}
#endif
