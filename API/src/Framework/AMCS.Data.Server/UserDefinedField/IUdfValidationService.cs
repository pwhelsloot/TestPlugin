namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.Collections.Generic;
  using AMCS.Data.Entity.UserDefinedField;

  internal interface IUdfValidationService
  {
    void Validate(IList<(string Namespace, Dictionary<string, object> Items)> udfValuesToWrite, Guid relatedResourceGuid, Type entityType, out Dictionary<IUdfField, IUdfResultObject> results);
  }
}