using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal interface IImportId
  {
    DataSet DataSet { get; }

    int Id { get; }

    bool IsSuccess { get; set; }

    int? NewId { get; set; }

    IDataSetRecord Record { get; }

    DataSetColumn Column { get; }
  }
}
