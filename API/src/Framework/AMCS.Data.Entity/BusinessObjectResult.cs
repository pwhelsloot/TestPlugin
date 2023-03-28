using System;
using System.Collections.Generic;

namespace AMCS.Data.Entity
{
  public class BusinessObjectResult
  {
    public BusinessObject BusinessObject { get; }

    public List<Type> Types { get; } = new List<Type>();

    public BusinessObjectResult(BusinessObject businessObject)
    {
      BusinessObject = businessObject;
    }
  }
}