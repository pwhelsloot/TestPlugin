namespace AMCS.ApiService
{
  public class RawJsonBody
  {
    public string JsonBody { get; }
    public RawJsonBody(string jsonBody)
    {
      JsonBody = jsonBody;
    }
  }
}