using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger
{
  public class ApiDocumentationConfiguration
  {
    public IList<ApiDocumentationVersion> Versions { get; } = new List<ApiDocumentationVersion>();

    public Type MarkdownDocumentationLocation { get; set; }

    public string ServiceRoot { get; set; }

    public string ErrorCodeTemplate { get; set; }
  }
}
