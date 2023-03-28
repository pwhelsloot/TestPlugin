namespace AMCS.Data.Server.SystemConfiguration
{
  public class ValidationResultFailure : IValidationResult
  {
    public string Message { get; }

    public ValidationResultFailure(string message)
    {
      Message = message;
    }
  }
}
