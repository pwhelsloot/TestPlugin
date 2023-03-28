using System;
using System.IO;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.Services
{
  public interface IDocumentAccessProvider
  {
    string Name { get; }

    Type EntityType { get; }

    void CreateAccessToken(ISessionToken userId, EntityObject entityObject, string accessToken, DateTime? expiration, IDataSession dataSession);

    DocumentAccessDocument GetDocument(ISessionToken userId, string accessToken, IDataSession dataSession);
  }
}
