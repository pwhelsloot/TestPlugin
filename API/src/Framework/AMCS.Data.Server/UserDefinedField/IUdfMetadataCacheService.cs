using System.Runtime.CompilerServices;
using AMCS.Data.Entity.UserDefinedField;
using AMCS.Data.Server.Services;

[assembly: InternalsVisibleTo("AMCS.PluginIntegration.Benchmarks")]
namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.Collections.Generic;

  internal interface IUdfMetadataCacheService : ICacheCoherentEntityService<UdfMetadataEntity>
  {
    IUdfMetadata GetUdfMetadata();

    IDictionary<(string Namespace, string FieldName), List<Type>> GetLinkedBusinessObjectTypes();
  }
}