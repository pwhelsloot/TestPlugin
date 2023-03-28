using System;
using System.Collections.Generic;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.Services
{
  public interface IBusinessObjectService
  {
    IList<BusinessObjectResult> GetAll();

    BusinessObjectResult Get(string objectName);

    BusinessObjectResult Get(Type type);
  }
}