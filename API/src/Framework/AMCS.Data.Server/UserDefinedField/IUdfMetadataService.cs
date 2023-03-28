using System.Runtime.CompilerServices;
using System.Collections.Generic;
using AMCS.Data.Entity.UserDefinedField;
using System;

[assembly: InternalsVisibleTo("AMCS.PluginIntegration.Benchmarks")]
[assembly: InternalsVisibleTo("AMCS.PluginIntegration.Plugin.Shared")]
namespace AMCS.Data.Server.UserDefinedField
{

  internal interface IUdfMetadataService
  {
    IUdfMetadata GetUdfMetadata();

    void SaveUdfMetadata(IList<UdfMetadataEntity> udfMetadata, string category, ISessionToken sessionToken,
      IDataSession dataSession);

    bool IsTypeValidForUdfMetadata(string @namespace, string fieldName, Type specifiedType);
  }
}