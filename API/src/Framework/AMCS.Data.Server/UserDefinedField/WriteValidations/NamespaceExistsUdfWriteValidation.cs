namespace AMCS.Data.Server.UserDefinedField.WriteValidations
{
  using System.Text;

  internal class NamespaceExistsUdfWriteValidation : IUdfWriteValidation
  {
    private readonly IUdfMetadataService udfMetadataService;

    public NamespaceExistsUdfWriteValidation(IUdfMetadataService udfMetadataService)
    {
      this.udfMetadataService = udfMetadataService;
    }

    public bool Validate(UdfWriteValidationParameters validateParameters, out string errorMessage)
    {
      var errorMessages = new StringBuilder("The following namespaces are not valid for UDF: ");
      var foundErrors = false;

      foreach (var udfValue in validateParameters.UdfValues)
      {
        var namespaceExists = false;
        foreach (var udfItem in udfMetadataService.GetUdfMetadata().Namespaces)
        {
          if (udfItem.Name != udfValue.Namespace) 
            continue;

          namespaceExists = true;
          break;
        }

        if (!namespaceExists)
        {
          errorMessages.Append(udfValue.Namespace);
          errorMessages.Append(";");
          foundErrors = true;
        }
      }

      if (foundErrors)
      {
        errorMessage = errorMessages.ToString();
        return false;
      }

      errorMessage = string.Empty;
      return true;
    }
  }
}