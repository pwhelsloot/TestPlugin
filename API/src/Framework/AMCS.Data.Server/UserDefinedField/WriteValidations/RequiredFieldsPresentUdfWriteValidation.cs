namespace AMCS.Data.Server.UserDefinedField.WriteValidations
{
  using System.Text;
  using Services;

  internal class RequiredFieldsPresentUdfWriteValidation : IUdfWriteValidation
  {
    private readonly IUdfMetadataService udfMetadataService;
    private readonly IBusinessObjectService businessObjectService;

    public RequiredFieldsPresentUdfWriteValidation(IUdfMetadataService udfMetadataService, IBusinessObjectService businessObjectService)
    {
      this.udfMetadataService = udfMetadataService;
      this.businessObjectService = businessObjectService;
    }

    public bool Validate(UdfWriteValidationParameters validateParameters, out string errorMessage)
    {
      var namespaces = udfMetadataService.GetUdfMetadata().Namespaces;
      var errorMessages = new StringBuilder("The following required fields were not present: ");
      var errorsFound = false;

      foreach (var udfValue in validateParameters.UdfValues)
      {
        foreach (var @namespace in namespaces)
        {
          if (udfValue.Namespace != @namespace.Name)
            continue;

          foreach (var existingUdfField in @namespace.Fields)
          {
            if (!existingUdfField.Required)
              continue;

            if (!udfMetadataService.IsTypeValidForUdfMetadata(existingUdfField.Namespace, existingUdfField.FieldName, validateParameters.EntityType))
              continue;

            var requiredFieldExists = false;
            foreach (var incomingUdfValue in udfValue.Items)
            {
              if (incomingUdfValue.Key != existingUdfField.FieldName)
                continue;

              requiredFieldExists = true;
              break;
            }

            if (!requiredFieldExists)
            {
              errorMessages.Append(existingUdfField.Namespace);
              errorMessages.Append(":");
              errorMessages.Append(existingUdfField.FieldName);
              errorMessages.Append(";");
              errorsFound = true;
            }
          }
        }
      }

      if (errorsFound)
      {
        errorMessage = errorMessages.ToString();
        return false;
      }

      errorMessage = string.Empty;
      return true;
    }
  }
}