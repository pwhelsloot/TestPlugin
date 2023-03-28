
namespace AMCS.Data.Server.Services
{
  public class DocumentAccessToken
  {
    public string AccessToken { get; }

    public IDocumentAccessProvider DocumentAccessProvider { get; }

    public string FileName { get; }

    public DocumentAccessToken(string accessToken, IDocumentAccessProvider documentAccessProvider, string fileName)
    {
      AccessToken = accessToken;
      DocumentAccessProvider = documentAccessProvider;
      FileName = fileName;
    }
  }
}
