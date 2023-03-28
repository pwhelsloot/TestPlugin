using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using AMCS.Data.Entity.UserDefinedField;

[assembly: InternalsVisibleTo("AMCS.ApiService")]
namespace AMCS.Data.Server.UserDefinedField
{
  internal interface IUdfDataService
  {
    Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(Guid relatedResourceGuid, IList<string> businessObjectNames, IList<string> namespaces, ISessionToken userId, IDataSession dataSession);
    Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(Guid relatedResourceGuid, IList<string> businessObjectNames, ISessionToken userId, IDataSession dataSession);
    Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(IList<Guid> relatedResourceGuids, IList<string> businessObjectNames, ISessionToken userId, IDataSession dataSession);
    Dictionary<Guid, Dictionary<IUdfField, IUdfResultObject>> Read(IList<Guid> relatedResourceGuids, IList<string> businessObjectNames, IList<string> namespaces, ISessionToken userId, IDataSession dataSession);
    
    void Write(Guid relatedResourceGuid, Type entityType, IList<(string Namespace, Dictionary<string, object> Items)> values, ISessionToken userId, IDataSession dataSession);

    void Delete(IList<Guid> relatedResourceGuids, ISessionToken userId, IDataSession dataSession);

    bool DataExistsFor(IDataSession dataSession, int udfFieldId);
  }
}