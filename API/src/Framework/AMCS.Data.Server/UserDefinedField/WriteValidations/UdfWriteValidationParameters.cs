namespace AMCS.Data.Server.UserDefinedField.WriteValidations
{
  using System;
  using System.Collections.Generic;

  internal class UdfWriteValidationParameters
  {
    public IList<(string Namespace, Dictionary<string, object> Items)> UdfValues { get; set; }

    public Type EntityType { get; set; }
  }
}