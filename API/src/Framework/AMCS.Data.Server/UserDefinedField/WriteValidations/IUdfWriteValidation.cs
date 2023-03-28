namespace AMCS.Data.Server.UserDefinedField.WriteValidations
{
  internal interface IUdfWriteValidation
  {
    bool Validate(UdfWriteValidationParameters validateParameters, out string errorMessage);
  }
}