using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal static class IDataSetImportProgressExtensions
  {
    public static void SetProgress(this IDataSetImportProgress self, int step, int total, string status)
    {
      self.SetProgress((double)step / total, status);
    }

    public static void SetProgress(this IDataSetImportProgress self, int step, int total, int subStep, int subTotal, string status)
    {
      self.SetProgress((step + ((double)subStep / subTotal)) / total, status);
    }
  }
}
