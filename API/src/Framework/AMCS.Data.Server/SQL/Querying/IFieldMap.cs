﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public interface IFieldMap
  {
    bool TryMap(string source, out string target);
  }
}
