using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.Services
{
  public interface IDocumentAccessManager
  {
    DocumentAccessToken CreateAccessToken(ISessionToken userId, EntityObject entityObject, DateTime? expiration, IDataSession dataSession, string fileName);

    string CreateAccessUrl(ISessionToken userId, EntityObject entityObject, DateTime? expiration, string fileName, IDataSession dataSession);

    DocumentAccessDocument GetDocument(ISessionToken userId, string type, string accessToken, IDataSession dataSession);

    string GetBundleUrl(ISessionToken userId, IList<DocumentAccessToken> tokens, DateTime? expiration, string fileName, IDataSession dataSession);
  }
}
