namespace AMCS.Data.Server.UserDefinedField.WriteValidations
{
  using System.Text;

  internal class AllFieldsExistUdfWriteValidation : IUdfWriteValidation
  {
    private readonly IUdfMetadataService udfMetadataService;

    public AllFieldsExistUdfWriteValidation(IUdfMetadataService udfMetadataService)
    {
      this.udfMetadataService = udfMetadataService;
    }

    public bool Validate(UdfWriteValidationParameters validateParameters, out string errorMessage)
    {
      var namespaces = this.udfMetadataService.GetUdfMetadata().Namespaces;
      var errorMessages = new StringBuilder($"The following UDFs were not found for type {validateParameters.EntityType.Name}: ");
      var foundErrors = false;

      foreach (var incomingUdfSet in validateParameters.UdfValues)
      {
        foreach (var @namespace in namespaces)
        {
          if (incomingUdfSet.Namespace != @namespace.Name)
            continue;

          foreach (var incomingUdfField in incomingUdfSet.Items)
          {
            var fieldExists = false;
            foreach (var udfField in @namespace.Fields)
            {
              if (incomingUdfField.Key != udfField.FieldName)
                continue;

              if (!udfMetadataService.IsTypeValidForUdfMetadata(incomingUdfSet.Namespace, incomingUdfField.Key, validateParameters.EntityType))
                continue;

              fieldExists = true;
              break;
            }

            if (!fieldExists)
            {
              errorMessages.Append(incomingUdfSet.Namespace);
              errorMessages.Append(":");
              errorMessages.Append(incomingUdfField.Key);
              errorMessages.Append(";");
              foundErrors = true;
            }
          }
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
