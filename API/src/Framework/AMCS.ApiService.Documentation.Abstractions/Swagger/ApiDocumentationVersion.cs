using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger
{
  public class ApiDocumentationVersion
  {
    public string Title { get; set; }

    public string Version { get; set; }

    public bool IsHidden { get; set; }
  }
}
