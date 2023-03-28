#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Elemos
{
  partial class XmlControllerProvider
  {
    private static readonly Type FileStreamResultType = typeof(FileStreamResult);
  }
}

#endif
