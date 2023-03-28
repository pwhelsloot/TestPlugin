﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public interface IDataEventsBuilderService
  {
    void Add(IDataEvents events);

    IDataEvents Build();
  }
}
